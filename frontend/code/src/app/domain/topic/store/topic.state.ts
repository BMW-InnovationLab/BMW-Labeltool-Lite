import { TranslateService } from '@ngx-translate/core'
import { Action, createSelector, Selector, State, StateContext } from '@ngxs/store'

import { ObjectClassService, TopicsService } from '@rcv/domain/labeltool-client'
import { NotificationService } from '@rcv/ui'
import { of } from 'rxjs'
import { catchError, tap } from 'rxjs/operators'
import {
  LoadObjectclasses,
  LoadTopics,
  SelectObjectclassFromLabelPage,
  SelectObjectclassFromObjectclassLoad,
  SelectTopic,
  SetChanges,
} from './topic.actions'
import { TopicStateModel } from './topic.model'

type Context = StateContext<TopicStateModel>

@State<TopicStateModel>({
  name: 'topic',
  defaults: {
    objectclasses: [],
    selectedObjectclass: undefined,
    selectedTopic: undefined,
    topics: [],
    hasChanges: false,
  },
})
export class TopicState {
  constructor(
    private objectClassService: ObjectClassService,
    private topicsService: TopicsService,
    private notificationService: NotificationService,
    private translate: TranslateService,
  ) {}

  @Selector()
  static hasChanges(state: TopicStateModel) {
    return state.hasChanges
  }

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

  static objectclass(id: number) {
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
  loadTopics(ctx: Context, action: LoadTopics) {
    return this.topicsService.getTopicsList().pipe(
      tap(topics => {
        ctx.patchState({ topics: topics })

        if (topics.length) {
          let initialTopic = topics.find(t => t.Id === action.initialTopicId)
          if (initialTopic == null) {
            initialTopic = topics[0]
          }

          ctx.dispatch([new SelectTopic(initialTopic)])
        }
      }),
      catchError(err => {
        this.notificationService.error(this.translate.instant('Could not load topics'), err)
        return of([])
      }),
    )
  }

  @Action(SelectTopic)
  selectTopic(ctx: Context, action: SelectTopic) {
    ctx.patchState({ selectedTopic: action.topic })
    ctx.dispatch([new LoadObjectclasses(action.topic)])
  }

  @Action(LoadObjectclasses)
  loadObjectclasses(ctx: Context, action: LoadObjectclasses) {
    return this.objectClassService.getObjectClasses(action.topic.Id).pipe(
      tap(objectclasses => {
        ctx.patchState({ objectclasses: objectclasses })

        if (objectclasses.length) {
          ctx.dispatch(new SelectObjectclassFromObjectclassLoad(objectclasses[0]))
        }
      }),
      catchError(err => {
        this.notificationService.error(this.translate.instant('Could not load object classes'), err)
        return of([])
      }),
    )
  }

  @Action([SelectObjectclassFromLabelPage, SelectObjectclassFromObjectclassLoad])
  selectObjectclass(ctx: Context, action: SelectObjectclassFromLabelPage) {
    ctx.patchState({ selectedObjectclass: action.objectclass })
  }

  @Action(SetChanges)
  setChanges(ctx: Context, action: SetChanges) {
    ctx.patchState({ hasChanges: action.hasChanges })
  }
}
