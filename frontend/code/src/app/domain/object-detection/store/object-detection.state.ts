import { ILabel } from '@domain/object-detection'
import { SetChanges } from '@domain/topic'
import { TranslateService } from '@ngx-translate/core'
import { Action, Selector, State, StateContext } from '@ngxs/store'
import { append, patch, removeItem, updateItem } from '@ngxs/store/operators'
import { NotificationService } from '@rcv/ui'
import { generateGuid } from 'src/app/core/libraries/guid'
import { LoadImage } from '../../image/store/image.actions'
import {
  AddLabel,
  AddLabelFromSuggestion,
  ChangeLabel,
  ChangeLabels,
  ChangeLabelsFromSave,
  CopyLabel,
  HoverLabel,
  RemoveLabel,
  RestoreLabels,
  SelectLabel,
  SetFill,
  SetStrokeWidth,
  ShowAllLabels,
  ToggleFill,
  ToggleLabelVisibility,
  ToggleVisibility,
} from './object-detection.actions'
import { ObjectDetectionStateModel } from './object-detection.model'

type Context = StateContext<ObjectDetectionStateModel>

@State<ObjectDetectionStateModel>({
  name: 'objectDetection',
  defaults: {
    allLabelsVisible: true,
    hovered: undefined,
    labels: [],
    strokeWidth: 4,
    fill: true,
  },
})
export class ObjectDetectionState {
  constructor(private notificationService: NotificationService, private translate: TranslateService) {}

  @Selector()
  static allLabelsVisible(state: ObjectDetectionStateModel) {
    return state.allLabelsVisible
  }

  @Selector()
  static fill(state: ObjectDetectionStateModel) {
    return state.fill
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

  @Selector()
  static strokeWidth(state: ObjectDetectionStateModel) {
    return state.strokeWidth
  }

  @Action(AddLabel)
  addLabel(ctx: Context, { label }: AddLabel) {
    ctx.setState(patch({ labels: append([{ ...label, isVisible: true, isNew: true }]) as any }))
    ctx.dispatch(new SetChanges())
  }

  @Action(AddLabelFromSuggestion)
  AddLabelFromSuggestion(ctx: Context, { label }: AddLabel) {
    ctx.setState(patch({ labels: append([{ ...label, isVisible: true }]) as any }))
    ctx.dispatch(new SetChanges(false))
  }

  @Action(ChangeLabel)
  changeLabel(ctx: Context, { element }: ChangeLabel) {
    const labels = ctx.getState().labels
    const oldLabel = labels.find(label => label.Id === element.Id)
    if (oldLabel == null) {
      return
    }

    let updatedLabel = { ...oldLabel, ...element }
    // reset prediction if label changed
    if (this.labelChanged(oldLabel, updatedLabel)) {
      updatedLabel = {
        ...updatedLabel,
        Confidence: undefined,
        Prediction: undefined,
        PredictionClass: undefined,
        SourceModel: undefined,
      }
    }

    ctx.setState(
      patch({
        labels: updateItem<ILabel>(label => (label ? label.Id : '') === updatedLabel.Id, updatedLabel),
      }),
    )
    ctx.dispatch(new SetChanges())
  }

  @Action(RemoveLabel)
  removeLabel(ctx: Context, { element }: RemoveLabel) {
    ctx.setState(
      patch({ labels: removeItem<ILabel>(label => (label ? label.Id : '') === element.Id) as any }),
    )
    ctx.dispatch(new SetChanges())
  }

  @Action([ChangeLabels, ChangeLabelsFromSave])
  changeLabels(ctx: Context, { labels }: ChangeLabels) {
    ctx.patchState({
      labels: labels.map(l => ({ ...l, isVisible: true, isSelected: false, isActive: false })),
    })
  }

  @Action(CopyLabel)
  copyLabel(ctx: Context, { label }: CopyLabel) {
    ctx.patchState({ labels: ctx.getState().labels.map(l => ({ ...l, isSelected: false })) })
    const newLabel = { ...label, Id: generateGuid(), isSelected: true, isVisible: true, isNew: true }
    ctx.setState(patch({ labels: append([newLabel]) as any }))
    ctx.dispatch(new SetChanges())
    this.notificationService.success(this.translate.instant('ACTIONS.LABEL_COPIED'))
  }

  @Action(HoverLabel)
  hoverLabel(ctx: Context, { label }: HoverLabel) {
    ctx.patchState({ hovered: label })
  }

  @Action(RestoreLabels)
  restoreLabels(ctx: Context, action: RestoreLabels) {
    ctx.dispatch([new LoadImage(action.topic, action.image.Index), new SetChanges(false)])
    this.notificationService.success(this.translate.instant('Successfully reset all changes'))
  }

  @Action(SelectLabel)
  selectLabel(ctx: Context, { label }: SelectLabel) {
    const state = ctx.getState()
    ctx.patchState({
      labels: state.labels.map(l => ({ ...l, isSelected: label ? l.Id === label.Id : false })),
    })
  }

  @Action(SetFill)
  setFill(ctx: Context, { fill }: SetFill) {
    ctx.patchState({ fill: fill })
  }

  @Action(SetStrokeWidth)
  setStrokeWidth(ctx: Context, { width }: SetStrokeWidth) {
    ctx.patchState({ strokeWidth: width })
  }

  @Action(ShowAllLabels)
  showAllLabels(ctx: Context) {
    this.showLabels(ctx, true)
  }

  @Action(ToggleFill)
  toggleFill(ctx: Context) {
    ctx.patchState({ fill: !ctx.getState().fill })
  }

  @Action(ToggleLabelVisibility)
  toggleLabelVisibility(ctx: Context, { label }: ToggleLabelVisibility) {
    ctx.setState(
      patch({
        labels: updateItem<ILabel>(l => (l ? l.Id : '') === label.Id, {
          ...label,
          isVisible: !label.isVisible,
        }),
      }),
    )
  }

  @Action(ToggleVisibility)
  toggleVisibility(ctx: Context) {
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

  private showLabels(ctx: Context, visible: boolean) {
    const state = ctx.getState()
    ctx.patchState({
      allLabelsVisible: visible,
      labels: state.labels.map(l => ({ ...l, isVisible: visible })),
    })
  }
}
