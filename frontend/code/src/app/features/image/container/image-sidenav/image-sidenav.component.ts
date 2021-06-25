import { HttpEventType } from '@angular/common/http'
import { Component } from '@angular/core'
import { MatDialog } from '@angular/material/dialog'
import { Router } from '@angular/router'

import { ConfirmModalComponent, DialogParameter } from '@common'
import { AbstractBaseComponent } from '@core'
import { LabelToolChangesService } from '@core/has-changes/label-tool-changes.service'
import { ConfigurationService } from '@domain/configuration'
import { ImageState, LoadImage, LoadInitialImage, RemoveImage, SetBrightness, SetZoom } from '@domain/image'
import { SetFill } from '@domain/object-detection'
import { SelectTopic, SetChanges, TopicState } from '@domain/topic'
import { TranslateService } from '@ngx-translate/core'
import { Select } from '@ngxs/store'
import { _ } from '@rcv/common'
import { AbstractRcvImageRepository, RcvLabelMode } from '@rcv/domain'
import { RcvImageLabelInterface, RcvTopicViewInterface } from '@rcv/domain/labeltool-client'
import { BusyIndicatorService } from '@rcv/ui'
import { Observable } from 'rxjs'
import { combineLatest, filter } from 'rxjs/operators'
import { HelpDialogComponent } from '../../components/help-dialog/help-dialog.component'
import { ImageDataModalComponent } from '../../components/image-data-modal/image-data-modal.component'
import { labelModePath, labelModeRoutes } from '../../label-mode-routing'
import {
  DialogParameter as TopicDialogParameter,
  TopicSelectDialogComponent,
} from '../topic-select-dialog/topic-select-dialog.component'

@Component({
  selector: 'rcv-image-sidenav',
  templateUrl: './image-sidenav.component.html',
  styleUrls: ['./image-sidenav.component.scss'],
})
export class ImageSidenavComponent extends AbstractBaseComponent {
  @Select(ImageState.brightness) brightness$: Observable<number>
  @Select(ImageState.mode) mode$: Observable<RcvLabelMode>
  @Select(ImageState.zoom) zoom$: Observable<number>
  @Select(TopicState.selectedTopic) selectedTopic$: Observable<RcvTopicViewInterface>

  image: RcvImageLabelInterface
  topic: RcvTopicViewInterface
  topics: RcvTopicViewInterface[] = []
  mode: RcvLabelMode
  routes = labelModeRoutes
  buttonUpload: boolean
  buttonDelete: boolean

  constructor(
    private dialog: MatDialog,
    private imageRepository: AbstractRcvImageRepository,
    private busyIndicatorService: BusyIndicatorService,
    protected translate: TranslateService,
    private router: Router,
    private labelToolChangesService: LabelToolChangesService,
    private configurationService: ConfigurationService,
  ) {
    super()
    this.labelToolChangesService.registerListeners()
    this.$s.add(
      this.store.select(ImageState.image).subscribe(i => (this.image = i as RcvImageLabelInterface)),
    )
    this.$s.add(this.store.select(TopicState.topics).subscribe(t => (this.topics = t)))
    this.$s.add(this.store.select(ImageState.mode).subscribe(m => (this.mode = m)))
    this.$s.add(this.selectedTopic$.subscribe(t => this.handleTopicChange(t)))
    // trigger reloads if label mode changes
    this.$s.add(
      this.selectedTopic$.pipe(combineLatest(this.mode$)).subscribe(v => this.handleTopicAndModeChange(v[0])),
    )
    const buttonUpload = this.configurationService.cfg.isUploadButtonVisible
    if (buttonUpload == null) {
      throw new Error('isUploadButtonVisible missing in config.json')
    }
    this.buttonUpload = buttonUpload
    const buttonDelete = this.configurationService.cfg.isDeleteButtonVisible
    if (buttonDelete == null) {
      throw new Error('isDeleteButtonVisible missing in config.json')
    }
    this.buttonDelete = buttonDelete
  }

  removeImage() {
    const dialog = this.dialog.open<ConfirmModalComponent, DialogParameter>(ConfirmModalComponent, {
      data: {
        confirmTitle: this.translate.instant('Delete image?'),
        confirmText: this.translate.instant('The image will be permanently deleted and cannot be restored'),
        acknowledgeText: this.translate.instant('ACTIONS.DELETE'),
        isDangerous: true,
      },
    })
    dialog
      .afterClosed()
      .pipe(filter(r => !!r))
      .subscribe(r => this.store.dispatch([new SetChanges(false), new RemoveImage(this.topic, this.image)]))
  }

  selectTopic() {
    this.dialog
      .open<TopicSelectDialogComponent, TopicDialogParameter>(TopicSelectDialogComponent, {
        width: '600px',
        data: { selectedTopic: this.topic, topics: this.topics },
      })
      .afterClosed()
      .pipe(filter(r => !!r))
      .subscribe(async topic => {
        if (!(await this.labelToolChangesService.hasChanges())) {
          this.store.dispatch([new SelectTopic(topic)])
        }
      })
  }

  decreaseZoom(zoom: number) {
    this.store.dispatch(new SetZoom(zoom - 0.1))
  }

  increaseZoom(zoom: number) {
    this.store.dispatch(new SetZoom(zoom + 0.1))
  }

  decreaseBrightness(brightness: number) {
    this.store.dispatch(new SetBrightness(brightness - 0.1))
  }

  increaseBrightness(brightness: number) {
    this.store.dispatch(new SetBrightness(brightness + 0.1))
  }

  showImageData() {
    this.dialog.open(ImageDataModalComponent, {
      data: {
        imageName: this.image.Id,
        imageWidth: this.image.Width,
        imageHeight: this.image.Height,
      },
      minWidth: '500px',
    })
  }

  showHelp(): void {
    this.dialog.open(HelpDialogComponent, {
      maxHeight: '100vh',
    })
  }

  uploadImages(images: File[]) {
    this.store.dispatch(new SetChanges(false))
    this.imageRepository.uploadImage(this.topic, images[0]).subscribe(
      event => {
        switch (event.type) {
          case HttpEventType.UploadProgress:
            if (typeof event.total === 'number') {
              const progress = event.total > 0 ? Math.round((100 * event.loaded) / event.total) : 0
              this.busyIndicatorService.setProgress(progress)
            }
            break
          case HttpEventType.Response:
            this.busyIndicatorService.setProgress()
            if (event.body != null) {
              this.notificationService.success(this.translate.instant('Image uploaded'))
              this.store.dispatch([new SetChanges(false), new LoadImage(this.topic, event.body.Index)])
            }
            break
        }
      },
      err => this.notificationService.error(this.translate.instant('Could not upload image!'), err),
    )
  }

  private handleTopicChange(topic: RcvTopicViewInterface) {
    if (!!topic) {
      this.router.navigate(['topics', topic.Id, labelModePath(this.mode)])
    }
  }

  private handleTopicAndModeChange(topic: RcvTopicViewInterface) {
    if (!!topic) {
      this.topic = topic
      this.store.dispatch([
        new LoadInitialImage(topic),
        new SetZoom(1),
        new SetBrightness(1),
        new SetFill(true),
      ])
    }
  }
}
