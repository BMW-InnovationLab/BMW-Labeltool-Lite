import { Component, OnInit, ViewChild } from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { LabelToolChangesService } from '@core/has-changes/label-tool-changes.service'

import { AbstractBaseComponent } from '@core/libraries/base-component'
import { ISize } from '@core/models/ISize'
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
import { SelectObjectclassFromLabelPage, TopicState } from '@domain/topic'
import { Select } from '@ngxs/store'
import {
  RcvImageLabelInterface,
  RcvObjectClassViewInterface,
  RcvTopicViewInterface,
} from '@rcv/domain/labeltool-client'
import { HasChangesService } from '@rcv/ui'
import { BoundingBoxEditorComponent, ElementData } from '@svg-editor'
import { EMPTY, Observable } from 'rxjs'
import { map } from 'rxjs/operators'

@Component({
  selector: 'rcv-label-page',
  templateUrl: './label-page.component.html',
  styleUrls: ['./label-page.component.scss'],
})
export class LabelPageComponent extends AbstractBaseComponent implements OnInit, HasChangesService {
  @ViewChild('editor', { static: false }) editor: BoundingBoxEditorComponent

  @Select(ObjectDetectionState.fill) fill$: Observable<boolean>
  @Select(ObjectDetectionState.hovered) hoveredLabel$: Observable<ILabel>
  @Select(ObjectDetectionState.labels) labels$: Observable<ILabel[]>
  @Select(ObjectDetectionState.selected) selectedLabel$: Observable<ILabel>
  @Select(ObjectDetectionState.strokeWidth) strokeWidth$: Observable<number>
  @Select(TopicState.objectclasses) objectclasses$: Observable<RcvObjectClassViewInterface[]>
  @Select(TopicState.selectedObjectclass) selectedObjectclass$: Observable<RcvObjectClassViewInterface>
  @Select(TopicState.selectedTopic) selectedTopic$: Observable<RcvTopicViewInterface>

  isSpatialDrawing$: Observable<boolean> = EMPTY

  image: RcvImageLabelInterface
  selectedObjectClass: RcvObjectClassViewInterface
  size: ISize = { width: 0, height: 0, naturalWidth: 0, naturalHeight: 0 }

  constructor(private route: ActivatedRoute, public changesService: LabelToolChangesService) {
    super()
    this.changesService.registerListeners()
    this.store.dispatch(new SelectMode(this.route.snapshot.data.mode))

    this.$s.add(this.selectedObjectclass$.subscribe(o => (this.selectedObjectClass = o)))
    this.$s.add(
      this.store.select(ImageState.image).subscribe(i => this.handleImage(i as RcvImageLabelInterface)),
    )
    this.$s.add(this.store.select(ImageState.imageSize).subscribe(i => this.setSize(i)))
  }

  ngOnInit(): void {
    this.isSpatialDrawing$ = this.selectedTopic$.pipe(map(topic => topic.LabelType === '3D'))
    // resize the labels if browser size has changed between navigation
    window.dispatchEvent(new Event('resize'))
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

  selectObjectClass(objectClass: RcvObjectClassViewInterface) {
    this.store.dispatch(new SelectObjectclassFromLabelPage(objectClass))
  }

  private handleImage(image: RcvImageLabelInterface) {
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
