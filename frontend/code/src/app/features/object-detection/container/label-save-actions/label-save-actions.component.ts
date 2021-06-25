import { Component, Inject } from '@angular/core'

import { AbstractBaseComponent } from '@core'
import { ImageState, LoadBlank } from '@domain/image'
import {
  AbstractObjectDetectionService,
  ChangeLabelsFromSave,
  ILabel,
  ObjectDetectionState,
} from '@domain/object-detection'
import { SetChanges, TopicState } from '@domain/topic'
import { TranslateService } from '@ngx-translate/core'
import { RcvImageLabelInterface, RcvTopicViewInterface } from '@rcv/domain/labeltool-client'
import { throwError } from 'rxjs'
import { catchError, tap } from 'rxjs/operators'

@Component({
  selector: 'rcv-label-save-actions',
  templateUrl: './label-save-actions.component.html',
  styleUrls: ['./label-save-actions.component.scss'],
})
export class LabelSaveActionsComponent extends AbstractBaseComponent {
  image: RcvImageLabelInterface
  labels: ILabel[]
  topic: RcvTopicViewInterface

  constructor(
    @Inject('IObjectDetectionService') private objectDetectionService: AbstractObjectDetectionService,
    protected translate: TranslateService,
  ) {
    super()
    this.$s.add(
      this.store.select(ImageState.image).subscribe(i => (this.image = i as RcvImageLabelInterface)),
    )
    this.$s.add(this.store.select(ObjectDetectionState.labels).subscribe(l => (this.labels = l)))
    this.$s.add(
      this.store.select(TopicState.selectedTopic).subscribe(t => (this.topic = t as RcvTopicViewInterface)),
    )
  }

  save() {
    this.doSave().subscribe(() => this.store.dispatch(new SetChanges(false)))
  }

  saveAndNext() {
    this.doSave().subscribe({
      complete: () =>
        this.store.dispatch([new SetChanges(false), new LoadBlank(this.topic, this.image.Index)]),
    })
  }

  private doSave() {
    return this.objectDetectionService.saveOrUpdate(this.topic, this.image.Id, this.labels).pipe(
      tap(labels => {
        this.notificationService.success(this.translate.instant('Saved labels'))
        this.store.dispatch(new ChangeLabelsFromSave(labels))
      }),
      catchError(e => {
        this.notificationService.error(this.translate.instant('Could not save labels'), e)
        return throwError(e)
      }),
    )
  }
}
