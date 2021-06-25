import { Component, HostListener, Inject, OnDestroy, OnInit } from '@angular/core'
import { ImageState } from '@domain/image'

import {
  AbstractObjectDetectionService,
  AddLabelFromSuggestion,
  ObjectDetectionState,
  ObjectDetectionStateModel,
  SetFill,
  SetStrokeWidth,
  ToggleFill,
} from '@domain/object-detection'
import { TopicState } from '@domain/topic'
import { TranslateService } from '@ngx-translate/core'
import { Select, Store } from '@ngxs/store'
import { RcvImageLabelInterface, RcvTopicViewInterface } from '@rcv/domain/labeltool-client'
import { RcvLabelViewInterface } from '@rcv/domain/labeltool-client/model/label-view.interface'
import { NotificationService } from '@rcv/ui'
import { Observable, Subscription } from 'rxjs'

@Component({
  selector: 'rcv-label-actions-bar',
  templateUrl: './label-actions-bar.component.html',
  styleUrls: ['./label-actions-bar.component.scss'],
})
export class LabelActionsBarComponent implements OnInit, OnDestroy {
  @Select(ImageState.image) image$: Observable<RcvImageLabelInterface>

  @Select(TopicState.selectedTopic) selectedTopic$: Observable<RcvTopicViewInterface>

  @Select() objectDetection$: Observable<ObjectDetectionStateModel>
  strokeWidths: number[] = [1, 2, 4, 8, 16]

  selectedImage: RcvImageLabelInterface
  selectedTopic: RcvTopicViewInterface

  protected readonly $s: Subscription = new Subscription()

  constructor(
    private store: Store,
    @Inject('IObjectDetectionService') private objectDetectionService: AbstractObjectDetectionService,
    private notificationService: NotificationService,
    private translate: TranslateService,
  ) {}

  ngOnDestroy(): void {
    this.$s.unsubscribe()
  }

  ngOnInit(): void {
    window.addEventListener('keydown', (evt: KeyboardEvent) => this.handleKeydown(evt), true)
    this.$s.add(this.image$.subscribe(i => (this.selectedImage = i)))
    this.$s.add(this.selectedTopic$.subscribe(t => (this.selectedTopic = t)))
  }

  @HostListener('window:keyup', ['$event'])
  handleKeyup(event: KeyboardEvent) {
    if (event.shiftKey && event.key === 'F') {
      this.store.dispatch(new ToggleFill())
    }
  }

  updateFill(fill: boolean) {
    this.store.dispatch(new SetFill(fill))
  }

  updateWidth(width: number) {
    this.store.dispatch(new SetStrokeWidth(width))
  }

  suggest() {
    if (!this.selectedImage) {
      return
    }
    this.objectDetectionService
      .suggest(this.selectedTopic, this.selectedImage.Id)
      .subscribe(
        result => this.handleSuggestResult(result),
        err =>
          this.notificationService.error(
            this.translate.instant('Suggest label is not possible due to an error'),
            err,
          ),
      )
  }

  private handleSuggestResult(labels: RcvLabelViewInterface[] | null) {
    if (!labels || labels.length === 0) {
      return
    }
    this.store.dispatch([...labels.map(label => new AddLabelFromSuggestion(label))])
  }

  private handleKeydown(event: KeyboardEvent) {
    if (event.ctrlKey) {
      switch (event.key) {
        case 'ArrowUp':
          this.increaseWidth(this.store.selectSnapshot(ObjectDetectionState.strokeWidth))
          break
        case 'ArrowDown':
          this.decreaseWidth(this.store.selectSnapshot(ObjectDetectionState.strokeWidth))
          break
        default:
        /* do nothing */
      }

      event.stopPropagation()
    }
  }

  private decreaseWidth(currentWidth: number) {
    const index = this.strokeWidths.indexOf(currentWidth)
    if (index !== -1 && index > 0) {
      this.updateWidth(this.strokeWidths[index - 1])
    }
  }

  private increaseWidth(currentWidth: number) {
    const index = this.strokeWidths.indexOf(currentWidth)
    if (index !== -1 && index < this.strokeWidths.length - 1) {
      this.updateWidth(this.strokeWidths[index + 1])
    }
  }
}
