import { Component } from '@angular/core'
import { Select } from '@ngxs/store'
import { Observable } from 'rxjs'

import { AbstractBaseComponent } from '@core'
import { ImageState } from '@domain/image'
import {
  ChangeLabels,
  HoverLabel,
  ILabel,
  ObjectDetectionState,
  RemoveLabel,
  RestoreLabels,
  SelectLabel,
  ShowAllLabels,
  ToggleLabelVisibility,
  ToggleVisibility,
} from '@domain/object-detection'
import { TopicState } from '@domain/topic'
import { RcvImageInterface, RcvObjectclassInterface, RcvTopicInterface } from '@rcv/domain'

@Component({
  templateUrl: './label-side-nav.component.html',
  styleUrls: ['./label-side-nav.component.scss'],
})
export class LabelSideNavComponent extends AbstractBaseComponent {
  @Select(ImageState.image) image$: Observable<RcvImageInterface>
  @Select(ObjectDetectionState.allLabelsVisible) allLabelsVisible$: Observable<boolean>
  @Select(ObjectDetectionState.labels) labels$: Observable<ILabel[]>
  @Select(ObjectDetectionState.hovered) hoveredLabel$: Observable<ILabel>
  @Select(ObjectDetectionState.selected) selectedLabel$: Observable<ILabel>
  @Select(TopicState.selectedTopic) selectedTopic$: Observable<RcvTopicInterface>
  @Select(TopicState.objectclasses) objectclasses$: Observable<RcvObjectclassInterface[]>

  image: RcvImageInterface
  labels: ILabel[] = []
  topic: RcvTopicInterface
  selected: ILabel

  constructor() {
    super()
    this.$s.add(
      this.image$.subscribe(i => {
        this.image = i
        this.store.dispatch(new ShowAllLabels())
      }),
    )
    this.$s.add(this.labels$.subscribe(l => (this.labels = l)))
    this.$s.add(this.selectedLabel$.subscribe(l => (this.selected = l)))
    this.$s.add(this.selectedTopic$.subscribe(t => (this.topic = t)))
  }

  select(label?: ILabel) {
    this.store.dispatch(new SelectLabel(label))
  }

  hover(label: ILabel) {
    this.store.dispatch(new HoverLabel(label))
  }

  toggleVisibility(label: ILabel) {
    this.store.dispatch(new ToggleLabelVisibility(label))
  }

  toggleVisibilityAll() {
    this.store.dispatch(new ToggleVisibility())
  }

  remove(label: ILabel) {
    this.store.dispatch(new RemoveLabel(label))
    if (this.selected && label.Id === this.selected.Id) {
      this.select()
    }
    this.notificationService.success('Label removed')
  }

  removeAll() {
    this.store.dispatch(new ChangeLabels([]))
    this.notificationService.success('All Labels Removed')
  }

  restore() {
    this.store.dispatch(new RestoreLabels(this.topic, this.image))
  }

  filterLabels(labels: ILabel[], objectClass: RcvObjectclassInterface): ILabel[] {
    return labels.filter(x => x.ObjectClassId === objectClass.Id)
  }
}
