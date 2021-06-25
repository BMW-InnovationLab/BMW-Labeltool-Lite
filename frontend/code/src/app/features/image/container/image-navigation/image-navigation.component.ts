import { HttpErrorResponse } from '@angular/common/http'
import { Component, HostListener } from '@angular/core'
import { MatSlider } from '@angular/material'

import { AbstractBaseComponent } from '@core'
import { LabelToolChangesService } from '@core/has-changes/label-tool-changes.service'
import { ImageState, SetImageResponse } from '@domain/image'
import { TopicState } from '@domain/topic'
import { TranslateService } from '@ngx-translate/core'
import { AbstractRcvImageRepository, RcvImageResponseInterface, RcvLabelMode } from '@rcv/domain'
import {
  RcvImageLabelInterface,
  RcvImageNavigationResultViewInterface,
  RcvTopicViewInterface,
} from '@rcv/domain/labeltool-client'
import { Observable } from 'rxjs'

@Component({
  selector: 'rcv-image-navigation',
  templateUrl: './image-navigation.component.html',
  styleUrls: ['./image-navigation.component.scss'],
})
export class ImageNavigationComponent extends AbstractBaseComponent {
  count: number
  image: RcvImageLabelInterface
  imageResponse: RcvImageResponseInterface
  mode: RcvLabelMode
  topic: RcvTopicViewInterface

  constructor(
    private imageRepository: AbstractRcvImageRepository,
    protected translate: TranslateService,
    private hasChangesService: LabelToolChangesService,
  ) {
    super()
    this.hasChangesService.registerListeners()
    this.$s.add(this.store.select(ImageState.count).subscribe(c => (this.count = c)))
    this.$s.add(
      this.store.select(ImageState.image).subscribe(i => (this.image = i as RcvImageLabelInterface)),
    )
    this.$s.add(
      this.store
        .select(ImageState.imageResponse)
        .subscribe(i => (this.imageResponse = i as RcvImageResponseInterface)),
    )
    this.$s.add(this.store.select(ImageState.mode).subscribe(m => (this.mode = m)))
    this.$s.add(
      this.store.select(TopicState.selectedTopic).subscribe(t => (this.topic = t as RcvTopicViewInterface)),
    )
  }

  async changeImage(slider: MatSlider) {
    if (await this.hasChangesService.hasChanges()) {
      slider.value = this.image.Index + 1
      return
    }
    this.imageRepository
      .image(this.mode, this.topic, slider.value - 1, 'Standard')
      .subscribe(this.handleImageResponse, this.handleImageError)
  }

  @HostListener('window:keyup', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (!event.shiftKey) {
      return
    }
    switch (event.key) {
      case 'Q':
        this.previousBlank()
        break
      case 'W':
        this.previous()
        break
      case 'E':
        this.next()
        break
      case 'R':
        this.nextBlank()
        break

      default:
      /* do nothing */
    }
  }

  async previous() {
    if (this.hasPrevious) {
      if (await this.hasChangesService.hasChanges()) {
        return
      }
      this.imageRepository
        .previous(this.mode, this.topic, this.image.Index, 'Standard')
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  async next() {
    if (this.hasNext) {
      if (await this.hasChangesService.hasChanges()) {
        return
      }
      this.imageRepository
        .next(this.mode, this.topic, this.image.Index, 'Standard')
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  async previousBlank() {
    if (this.hasPreviousBlank) {
      if (await this.hasChangesService.hasChanges()) {
        return
      }
      this.imageRepository
        .previousBlank(this.mode, this.topic, this.image.Index, 'Standard')
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  async nextBlank() {
    if (this.hasNextBlank) {
      if (await this.hasChangesService.hasChanges()) {
        return
      }
      this.imageRepository
        .nextBlank(this.mode, this.topic, this.image.Index, 'Standard')
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  get previousBlankTooltip(): Observable<string> {
    return this.translate.get('IMAGE_NAVIGATION.PREVIOUS_UNLABELED_IMAGE')
  }

  get nextBlankTooltip(): Observable<string> {
    return this.translate.get('IMAGE_NAVIGATION.NEXT_UNLABELED_IMAGE')
  }

  get hasNext() {
    return this.imageResponse.HasNext
  }

  get hasNextBlank() {
    return this.imageResponse.HasNextBlank
  }

  get hasPrevious() {
    return this.imageResponse.HasPrevious
  }

  get hasPreviousBlank() {
    return this.imageResponse.HasPreviousBlank
  }

  async updateIndex(element: HTMLInputElement) {
    let value = Number(element.value)

    if (value <= 0) {
      value = 1
    } else if (value > this.count) {
      value = this.count
    }

    if (value === this.image.Index + 1) {
      return
    }

    element.value = value.toString()
    await this.changeImage({ value } as MatSlider)
  }

  handleImageResponse = (response: RcvImageNavigationResultViewInterface) => {
    this.store.dispatch(new SetImageResponse(this.topic, response))
  }

  handleImageError = (err: HttpErrorResponse) => {
    this.notificationService.error('Could not navigate image!', err)
  }
}
