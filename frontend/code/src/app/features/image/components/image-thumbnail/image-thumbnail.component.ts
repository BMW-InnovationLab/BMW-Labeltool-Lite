import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core'
import { DomSanitizer } from '@angular/platform-browser'

import { RcvImageThumbnailInterface } from '@domain/image'

@Component({
  selector: 'rcv-image-thumbnail',
  templateUrl: './image-thumbnail.component.html',
  styleUrls: ['./image-thumbnail.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ImageThumbnailComponent implements OnInit, OnDestroy {
  @Input() thumbnail: RcvImageThumbnailInterface
  @Input() selectedIndex: number

  @Output() loadThumbnail = new EventEmitter<number>()

  @Output() navigateToIndex = new EventEmitter<number>()

  startLoadingImage = false

  private timeoutId: number = null

  constructor(private sanitizer: DomSanitizer, private cd: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (!this.thumbnail.thumbnailUrl) {
      this.timeoutId = setTimeout(() => {
        this.startLoadingImage = true
        this.loadThumbnail.emit(this.thumbnail.index)
        this.cd.markForCheck()
      }, 750)
    }
  }

  ngOnDestroy(): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId)
      this.timeoutId = null
    }
  }
  thumbnailUrl(url: string) {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url)
  }
}
