import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
} from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { Select } from '@ngxs/store'
import { Observable } from 'rxjs'

import { AbstractBaseComponent } from '@core'
import { ImageReady, ImageState, SelectMode, SetImageSize } from '@domain/image'
import { LoadTopics } from '@domain/topic'
import { RcvImageInterface } from '@rcv/domain'

@Component({
  selector: 'rcv-image-page',
  templateUrl: './image-page.component.html',
  styleUrls: ['./image-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ImagePageComponent extends AbstractBaseComponent implements OnInit {
  @ViewChild('container') container: ElementRef
  @ViewChild('editor') editor: ElementRef

  @Select(ImageState.image) image$: Observable<RcvImageInterface>
  @Select(ImageState.imageReady) imageReady$: Observable<boolean>

  private image: HTMLImageElement

  constructor(private route: ActivatedRoute) {
    super()
    if (this.route.snapshot.firstChild) {
      this.store.dispatch(new SelectMode(this.route.snapshot.firstChild.data.mode))
    }
  }

  ngOnInit() {
    this.store.dispatch(new LoadTopics(+this.route.snapshot.params.topicId))
  }

  @HostListener('window:resize')
  onResize() {
    this.setSize()
  }

  imageLoaded(event: Event) {
    this.image = event.target as HTMLImageElement
    this.store.dispatch(new ImageReady(true))
    this.image.style.width = 'auto'
    this.image.style.height = 'auto'
    this.image.style.maxHeight = 'calc(100vh - 2rem)'
    this.setSize()
  }

  private setSize() {
    if (!!this.image) {
      const elementRect = this.image.getBoundingClientRect()
      this.editor.nativeElement.style.width = `${elementRect.width}px`
      this.editor.nativeElement.style.height = `${elementRect.height}px`
      this.store.dispatch(new SetImageSize([elementRect.width, elementRect.height]))
    }
  }
}
