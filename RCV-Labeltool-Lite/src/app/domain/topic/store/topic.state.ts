import { Navigate } from '@ngxs/router-plugin'
import { Action, createSelector, Selector, State, StateContext, Store } from '@ngxs/store'
import produce from 'immer'
import { of } from 'rxjs'
import { catchError, tap } from 'rxjs/operators'

import { AbstractRcvObjectclassRepository, AbstractRcvTopicRepository, RcvLabelMode } from '@rcv/domain'
import { NotificationService } from '@rcv/ui'
import { ImageState } from '../../image'
import { LoadObjectclasses, LoadTopics, SelectObjectclass, SelectTopic } from './topic.actions'
import { TopicStateModel } from './topic.model'

@State<TopicStateModel>({
  name: 'topic',
  defaults: {
    objectclasses: [],
    selectedObjectclass: undefined,
    selectedTopic: undefined,
    topics: [],
  },
})
export class TopicState {
  constructor(
    private objectclassRepository: AbstractRcvObjectclassRepository,
    private topicRepository: AbstractRcvTopicRepository,
    private notificationService: NotificationService,
    private store: Store,
  ) {}

  @Selector()
  static topics(state: TopicStateModel) {
    return state.topics
  }

  @Selector()
  static selectedTopic(state: TopicStateModel) {
    return state.selectedTopic
  }

  @Selector()
  static objectclasses(state: TopicStateModel) {
    return state.objectclasses
  }

  static objectclass(id: string) {
    return createSelector(
      [TopicState],
      (state: TopicStateModel) => state.objectclasses.find(objectclass => objectclass.Id === id),
    )
  }

  @Selector()
  static selectedObjectclass(state: TopicStateModel) {
    return state.selectedObjectclass
  }

  @Action(LoadTopics)
  loadTopics(ctx: StateContext<TopicStateModel>, action: LoadTopics) {
    return this.topicRepository.Topics$.pipe(
      tap(topics => {
        ctx.setState(
          produce(ctx.getState(), draft => {
            draft.topics = topics
          }),
        )
        if (topics.length) {
          let initialTopic = topics.find(t => t.Id === action.initialTopicId)
          if (initialTopic == null) {
            initialTopic = topics[0]
          }

          ctx.dispatch([new SelectTopic(initialTopic)])
        }
      }),
      catchError(err => {
        this.notificationService.error('Konnte Topics nicht laden', err)
        return of([])
      }),
    )
  }

  @Action(SelectTopic)
  selectTopic(ctx: StateContext<TopicStateModel>, action: SelectTopic) {
    ctx.setState(
      produce(ctx.getState(), draft => {
        draft.selectedTopic = action.topic
      }),
    )

    const mode =
      this.store.selectSnapshot(ImageState.mode) === RcvLabelMode.ObjectDetection
        ? 'object-detection'
        : 'image-segmentation'
    ctx.dispatch([new LoadObjectclasses(action.topic), new Navigate(['topics', action.topic.Id, mode])])
  }

  @Action(LoadObjectclasses)
  loadObjectclasses(ctx: StateContext<TopicStateModel>, action: LoadObjectclasses) {
    return this.objectclassRepository.Objectclasses$(action.topic).pipe(
      tap(objectclasses => {
        ctx.setState(
          produce(ctx.getState(), draft => {
            draft.objectclasses = objectclasses
          }),
        )
        if (objectclasses.length) {
          ctx.dispatch(new SelectObjectclass(objectclasses[0]))
        }
      }),
      catchError(err => {
        this.notificationService.error('Cannot Load ObjectClass', err)
        return of([])
      }),
    )
  }

  @Action(SelectObjectclass)
  selectObjectclass(ctx: StateContext<TopicStateModel>, action: SelectObjectclass) {
    ctx.setState(
      produce(ctx.getState(), draft => {
        draft.selectedObjectclass = action.objectclass
      }),
    )
  }
}
