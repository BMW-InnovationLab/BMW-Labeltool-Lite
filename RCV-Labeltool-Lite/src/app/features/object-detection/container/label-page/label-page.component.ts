import { Component, ViewChild } from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { Select } from '@ngxs/store'
import { Observable } from 'rxjs'

import { AbstractBaseComponent } from '@core/libraries/base-component'
import { ImageState, SelectMode } from '@domain/image'
import {
  AddLabel,
  ChangeLabel,
  HoverLabel,
  ILabel,
  ObjectDetectionState,
  RemoveLabel,
  SelectLabel,
} from '@domain/object-detection'
import { SelectObjectclass, TopicState } from '@domain/topic'
import { RcvImageInterface, RcvObjectclassInterface } from '@rcv/domain'
import { BoundingBoxEditorComponent, ElementData } from '@svg-editor'

@Component({
  selector: 'rcv-label-page',
  templateUrl: './label-page.component.html',
  styleUrls: ['./label-page.component.scss'],
})
export class LabelPageComponent extends AbstractBaseComponent {
  @ViewChild('editor') editor: BoundingBoxEditorComponent

  @Select(ObjectDetectionState.hovered) hoveredLabel$: Observable<ILabel>
  @Select(ObjectDetectionState.labels) labels$: Observable<ILabel[]>
  @Select(ObjectDetectionState.selected) selectedLabel$: Observable<ILabel>
  @Select(TopicState.objectclasses) objectclasses$: Observable<RcvObjectclassInterface[]>
  @Select(TopicState.selectedObjectclass) selectedObjectclass$: Observable<RcvObjectclassInterface>

  image: RcvImageInterface
  selectedObjectClass: RcvObjectclassInterface
  size: any = { width: 0, height: 0, naturalWidth: 0, naturalHeight: 0 }

  constructor(private route: ActivatedRoute) {
    super()
    this.store.dispatch(new SelectMode(this.route.snapshot.data.mode))

    this.$s.add(this.selectedObjectclass$.subscribe(o => (this.selectedObjectClass = o)))
    this.$s.add(this.store.select(ImageState.image).subscribe(i => this.handleImage(i as any)))
    this.$s.add(this.store.select(ImageState.imageSize).subscribe(i => this.setSize(i)))
  }

  elementAdded(element: ElementData) {
    this.store.dispatch(
      new AddLabel({
        ...element,
        ObjectClassId: this.selectedObjectClass.Id,
        ObjectClassName: this.selectedObjectClass.Name,
        isActive: false,
      } as ILabel),
    )
  }

  elementChanged(element: ElementData) {
    this.store.dispatch(new ChangeLabel(element))
  }

  elementRemoved(element: ElementData) {
    this.store.dispatch(new RemoveLabel(element))
  }

  selectLabel(label: ILabel) {
    this.store.dispatch(new SelectLabel(label))
  }

  hoverLabel(label: ILabel) {
    this.store.dispatch(new HoverLabel(label))
  }

  selectObjectClass(objectClass: RcvObjectclassInterface) {
    this.store.dispatch(new SelectObjectclass(objectClass))
  }

  private handleImage(image: RcvImageInterface) {
    this.image = image
    if (this.editor != null) {
      this.editor.clear()
    }
  }

  private setSize([width, height]: [number, number]) {
    this.size = {
      naturalHeight: this.image.Height,
      naturalWidth: this.image.Width,
      height: height,
      width: width,
    }
  }
}
