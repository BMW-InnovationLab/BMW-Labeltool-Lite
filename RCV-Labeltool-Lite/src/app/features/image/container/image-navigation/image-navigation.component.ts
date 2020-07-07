import { HttpErrorResponse } from '@angular/common/http'
import { Component } from '@angular/core'

import { AbstractBaseComponent } from '@core'
import { ImageState, SetImageResponse } from '@domain/image'
import { TopicState } from '@domain/topic'
import {
  AbstractRcvImageRepository,
  RcvImageInterface,
  RcvImageResponseInterface,
  RcvLabelMode,
  RcvTopicInterface,
} from '@rcv/domain'

@Component({
  selector: 'rcv-image-navigation',
  templateUrl: './image-navigation.component.html',
  styleUrls: ['./image-navigation.component.scss'],
})
export class ImageNavigationComponent extends AbstractBaseComponent {
  count: number
  image: RcvImageInterface
  imageResponse: RcvImageResponseInterface
  mode: RcvLabelMode
  topic: RcvTopicInterface

  constructor(private imageRepository: AbstractRcvImageRepository) {
    super()
    this.$s.add(this.store.select(ImageState.count).subscribe(c => (this.count = c)))
    this.$s.add(this.store.select(ImageState.image).subscribe(i => (this.image = i as any)))
    this.$s.add(this.store.select(ImageState.imageResponse).subscribe(i => (this.imageResponse = i as any)))
    this.$s.add(this.store.select(ImageState.mode).subscribe(m => (this.mode = m)))
    this.$s.add(this.store.select(TopicState.selectedTopic).subscribe(t => (this.topic = t as any)))
  }

  changeImage(value: number) {
    this.imageRepository
      .image(this.mode, this.topic, value - 1)
      .subscribe(this.handleImageResponse, this.handleImageError)
  }

  previous() {
    if (this.hasPrevious) {
      this.imageRepository
        .previous(this.mode, this.topic, this.image.Index, false)
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  next() {
    if (this.hasNext) {
      this.imageRepository
        .next(this.mode, this.topic, this.image.Index, false)
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  previousBlank() {
    if (this.hasPreviousBlank) {
      this.imageRepository
        .previousBlank(this.mode, this.topic, this.image.Index, false)
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
  }

  nextBlank() {
    if (this.hasNextBlank) {
      this.imageRepository
        .nextBlank(this.mode, this.topic, this.image.Index, false)
        .subscribe(this.handleImageResponse, this.handleImageError)
    }
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

  updateIndex(element: HTMLInputElement) {
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
    this.changeImage(value)
  }

  handleImageResponse = (response: RcvImageResponseInterface) => {
    this.store.dispatch([new SetImageResponse(response)])
  }

  handleImageError = (err: HttpErrorResponse) => {
    this.notificationService.error('Could not navigate image!', err)
  }
}
