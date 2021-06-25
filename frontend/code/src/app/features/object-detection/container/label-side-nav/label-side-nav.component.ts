import { Component, HostListener } from '@angular/core'

import { AbstractBaseComponent } from '@core'
import { LabelToolChangesService } from '@core/has-changes/label-tool-changes.service'
import { ImageState } from '@domain/image'
import {
  ChangeLabels,
  CopyLabel,
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
import { SetChanges, TopicState } from '@domain/topic'
import { TranslateService } from '@ngx-translate/core'
import { Select } from '@ngxs/store'
import {
  RcvImageLabelInterface,
  RcvObjectClassViewInterface,
  RcvTopicViewInterface,
} from '@rcv/domain/labeltool-client'
import { HasChangesService } from '@rcv/ui'
import { Observable } from 'rxjs'

@Component({
  templateUrl: './label-side-nav.component.html',
  styleUrls: ['./label-side-nav.component.scss'],
})
export class LabelSideNavComponent extends AbstractBaseComponent implements HasChangesService {
  @Select(ImageState.image) image$: Observable<RcvImageLabelInterface>
  @Select(ObjectDetectionState.allLabelsVisible) allLabelsVisible$: Observable<boolean>
  @Select(ObjectDetectionState.labels) labels$: Observable<ILabel[]>
  @Select(ObjectDetectionState.hovered) hoveredLabel$: Observable<ILabel>
  @Select(ObjectDetectionState.selected) selectedLabel$: Observable<ILabel>
  @Select(TopicState.selectedTopic) selectedTopic$: Observable<RcvTopicViewInterface>
  @Select(TopicState.objectclasses) objectclasses$: Observable<RcvObjectClassViewInterface[]>

  image: RcvImageLabelInterface
  labels: ILabel[] = []
  topic: RcvTopicViewInterface
  selected: ILabel

  constructor(protected translate: TranslateService, public changesService: LabelToolChangesService) {
    super()
    this.changesService.registerListeners()
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

  @HostListener('window:keyup', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (!event.shiftKey) {
      return
    }

    switch (event.key) {
      case 'V':
        if (this.selected) {
          this.copy(this.selected)
        }
        break

      default:
      /* do nothing */
    }
  }

  copy(label: ILabel) {
    this.store.dispatch(new CopyLabel(label))
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
    this.notificationService.success(this.translate.instant('Removed label'))
  }

  removeAll() {
    this.store.dispatch([new ChangeLabels([]), new SetChanges()])
    this.notificationService.success(this.translate.instant('Removed all labels'))
  }

  restore() {
    this.store.dispatch(new RestoreLabels(this.topic, this.image))
  }

  filterLabels(labels: ILabel[], objectClass: RcvObjectClassViewInterface): ILabel[] {
    return labels.filter(x => x.ObjectClassId === objectClass.Id)
  }
}
