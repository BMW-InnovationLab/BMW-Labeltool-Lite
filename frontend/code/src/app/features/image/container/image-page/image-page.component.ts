import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
} from '@angular/core'
import { ActivatedRoute } from '@angular/router'

import { AbstractBaseComponent } from '@core'
import { ImageReady, ImageState, SelectMode, SetDefaultImageSize, SetImageSize, SetZoom } from '@domain/image'
import { LabelImageService } from '@domain/image/services/label-image.service'
import { LoadTopics } from '@domain/topic'
import { Select } from '@ngxs/store'
import { RcvImageLabelInterface } from '@rcv/domain/labeltool-client'
import normalizeWheel from 'normalize-wheel'
import { EMPTY, Observable } from 'rxjs'
import { first, switchMap } from 'rxjs/operators'

@Component({
  selector: 'rcv-image-page',
  templateUrl: './image-page.component.html',
  styleUrls: ['./image-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ImagePageComponent extends AbstractBaseComponent implements OnInit {
  @ViewChild('container', { static: true }) container: ElementRef
  @ViewChild('editor', { static: false }) editor: ElementRef
  @ViewChild('image', { static: false }) imageElement: ElementRef

  @Select(ImageState.brightness) brightness$: Observable<number>
  @Select(ImageState.image) image$: Observable<RcvImageLabelInterface>
  @Select(ImageState.imageReady) imageReady$: Observable<boolean>
  @Select(ImageState.defaultImageSize) private defaultImageSize$: Observable<[number, number]>

  imageUrl$: Observable<string> = EMPTY

  private zoomLevel = 1
  private image: HTMLImageElement

  constructor(private route: ActivatedRoute, private labelImageService: LabelImageService) {
    super()
    if (this.route.snapshot.firstChild) {
      this.store.dispatch(new SelectMode(this.route.snapshot.firstChild.data.mode))
    }

    this.$s.add(this.store.select(ImageState.mode).subscribe(() => this.handleModeChange()))
    this.$s.add(this.store.select(ImageState.image).subscribe(() => this.resetSize()))
    this.$s.add(this.store.select(ImageState.zoom).subscribe(z => this.setZoom(z)))
    this.$s.add(this.store.select(ImageState.imageResponse).subscribe(() => this.checkLoadingState()))
  }

  get topicId(): number {
    return +this.route.snapshot.params.topicId
  }

  ngOnInit() {
    this.store.dispatch(new LoadTopics(this.topicId))
    this.imageUrl$ = this.image$.pipe(
      switchMap(image => this.labelImageService.getImageUrl(this.topicId, image)),
    )
  }

  @HostListener('window:resize')
  onResize() {
    // give it some time to update the layout
    setTimeout(() => {
      this.resetSize()
      this.resetImageSize()
      this.setZoom(this.zoomLevel)
    }, 0)
  }

  @HostListener('wheel', ['$event'])
  onScroll(event: WheelEvent) {
    if (!event.shiftKey) {
      return
    }
    event.preventDefault()

    const wheel = normalizeWheel(event)

    this.zoomLevel -= wheel.spinY / 10
    this.zoomLevel = Math.max(1, this.zoomLevel)
    this.store.dispatch(new SetZoom(this.zoomLevel))
  }

  async setZoom(zoom: number) {
    this.zoomLevel = zoom

    if (!this.isImageValid()) {
      return
    }

    let defaultImageSize = await this.defaultImageSize$.pipe(first()).toPromise()

    const elementRect = this.image.getBoundingClientRect()
    if (defaultImageSize[0] === 0 && defaultImageSize[1] === 0) {
      defaultImageSize = [elementRect.width, elementRect.height]
      this.store.dispatch(new SetDefaultImageSize(defaultImageSize))
    }

    this.image.style.maxHeight = 'none'
    this.image.style.maxWidth = 'none'
    this.image.style.width = `${defaultImageSize[0] * this.zoomLevel}px`
    this.image.style.height = `${defaultImageSize[1] * this.zoomLevel}px`
    this.setSize()
  }

  imageLoaded(event: Event) {
    this.image = event.target as HTMLImageElement
    this.store.dispatch(new ImageReady(true))

    this.resetImageSize()
    this.setSize()
    this.setZoom(this.zoomLevel)
  }

  imageStyle(brightness: number) {
    return { filter: `brightness(${brightness})` }
  }

  private isImageValid() {
    if (!this.image) {
      return false
    }
    const rect = this.image.getBoundingClientRect()
    return rect.width > 0 && rect.height > 0
  }

  private checkLoadingState() {
    setTimeout(() => {
      if (this.imageElement != null && this.imageElement.nativeElement.complete) {
        this.store.dispatch(new ImageReady(true))
      }
    })
  }

  private setSize() {
    if (this.isImageValid()) {
      const elementRect = this.image.getBoundingClientRect()
      this.editor.nativeElement.style.width = `${elementRect.width}px`
      this.editor.nativeElement.style.height = `${elementRect.height}px`
      this.store.dispatch(new SetImageSize([elementRect.width, elementRect.height]))
    }
  }

  private handleModeChange() {
    this.store.dispatch(new SetZoom(1))
  }

  private resetImageSize() {
    if (this.isImageValid()) {
      this.image.style.width = 'auto'
      this.image.style.maxWidth = this.container.nativeElement.getBoundingClientRect().width + 'px'
      this.image.style.height = 'auto'
      this.image.style.maxHeight = 'calc(100vh - 2rem)'
    }
  }

  private resetSize() {
    this.store.dispatch(new SetDefaultImageSize([0, 0]))
  }
}
