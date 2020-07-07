import { produce } from '@ngxs-labs/immer-adapter'
import { Action, Selector, State, StateContext } from '@ngxs/store'

import { ILabel } from '@domain/object-detection'
import { NotificationService } from '@rcv/ui'
import { LoadImage } from '../../image/store/image.actions'
import {
  AddLabel,
  ChangeLabel,
  ChangeLabels,
  HoverLabel,
  RemoveLabel,
  RestoreLabels,
  SelectLabel,
  ShowAllLabels,
  ToggleLabelVisibility,
  ToggleVisibility,
} from './object-detection.actions'
import { ObjectDetectionStateModel } from './object-detection.model'

@State<ObjectDetectionStateModel>({
  name: 'objectDetection',
  defaults: {
    allLabelsVisible: true,
    hovered: undefined,
    labels: [],
  },
})
export class ObjectDetectionState {
  constructor(private notificationService: NotificationService) {}

  @Selector()
  static allLabelsVisible(state: ObjectDetectionStateModel) {
    return state.allLabelsVisible
  }

  @Selector()
  static hovered(state: ObjectDetectionStateModel) {
    return state.hovered
  }

  @Selector()
  static labels(state: ObjectDetectionStateModel) {
    return state.labels
  }

  @Selector()
  static selected(state: ObjectDetectionStateModel) {
    return state.labels.find(l => !!l.isSelected)
  }

  @Action(AddLabel)
  addLabel(ctx: StateContext<ObjectDetectionStateModel>, { label }: AddLabel) {
    produce(ctx, (draft: ObjectDetectionStateModel) => {
      draft.labels.push({ ...label, isVisible: true })
    })
  }

  @Action(ChangeLabel)
  changeLabel(ctx: StateContext<ObjectDetectionStateModel>, action: ChangeLabel) {
    produce(ctx, draft => {
      const index = draft.labels.findIndex(label => label.Id === action.element.Id)
      if (index !== -1) {
        const oldLabel = draft.labels[index]
        const updatedLabel = { ...oldLabel, ...action.element }
        if (this.labelChanged(oldLabel, updatedLabel)) {
          draft.labels[index] = {
            ...updatedLabel,
            Confidence: undefined,
            Prediction: undefined,
            PredictionClass: undefined,
          } as ILabel
        } else {
          draft.labels[index] = updatedLabel as ILabel
        }
      }
    })
  }

  @Action(RemoveLabel)
  removeLabel(ctx: StateContext<ObjectDetectionStateModel>, action: RemoveLabel) {
    produce(ctx, draft => {
      draft.labels.splice(draft.labels.findIndex(l => l.Id === action.element.Id), 1)
    })
  }

  @Action(ChangeLabels)
  changeLabels(ctx: StateContext<ObjectDetectionStateModel>, action: ChangeLabels) {
    ctx.patchState({ labels: action.labels })
  }

  @Action(HoverLabel)
  hoverLabel(ctx: StateContext<ObjectDetectionStateModel>, action: HoverLabel) {
    ctx.patchState({ hovered: action.label })
  }

  @Action(RestoreLabels)
  restoreLabels(ctx: StateContext<ObjectDetectionStateModel>, action: RestoreLabels) {
    ctx.dispatch(new LoadImage(action.topic, action.image.Index))
    this.notificationService.success('Successfully reset all changes')
  }

  @Action(SelectLabel)
  selectLabel(ctx: StateContext<ObjectDetectionStateModel>, action: SelectLabel) {
    produce(ctx, draft => {
      draft.labels.forEach(l => (l.isSelected = action.label ? l.Id === action.label.Id : false))
    })
  }

  @Action(ShowAllLabels)
  showAllLabels(ctx: StateContext<ObjectDetectionStateModel>) {
    this.showLabels(ctx, true)
  }

  @Action(ToggleLabelVisibility)
  toggleLabelVisibility(ctx: StateContext<ObjectDetectionStateModel>, action: ToggleLabelVisibility) {
    produce(ctx, draft => {
      const label = draft.labels.find(l => l.Id === action.label.Id)
      if (label) {
        label.isVisible = !label.isVisible
      }
    })
  }

  @Action(ToggleVisibility)
  toggleVisibility(ctx: StateContext<ObjectDetectionStateModel>) {
    this.showLabels(ctx, !ctx.getState().allLabelsVisible)
  }

  private labelChanged(a: ILabel, b: ILabel): boolean {
    return !(
      a.Bottom === b.Bottom &&
      a.Left === b.Left &&
      a.ObjectClassId === b.ObjectClassId &&
      a.Right === b.Right &&
      a.Top === b.Top
    )
  }

  private showLabels(ctx: StateContext<ObjectDetectionStateModel>, visible: boolean) {
    produce(ctx, draft => {
      draft.allLabelsVisible = visible
      draft.labels.forEach(l => (l.isVisible = draft.allLabelsVisible))
    })
  }
}
