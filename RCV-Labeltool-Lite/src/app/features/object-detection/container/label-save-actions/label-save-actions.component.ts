import { Component, Inject } from '@angular/core'

import { AbstractBaseComponent } from '@core'
import { ImageState } from '@domain/image'
import { ILabel, ObjectDetectionState } from '@domain/object-detection'
import { AbstractObjectDetectionService } from '@domain/object-detection'
import { TopicState } from '@domain/topic'
import { RcvImageInterface, RcvTopicInterface } from '@rcv/domain'

@Component({
  selector: 'rcv-label-save-actions',
  templateUrl: './label-save-actions.component.html',
  styleUrls: ['./label-save-actions.component.scss'],
})
export class LabelSaveActionsComponent extends AbstractBaseComponent {
  image: RcvImageInterface
  labels: ILabel[]
  topic: RcvTopicInterface

  constructor(
    @Inject('IObjectDetectionService') private objectDetectionService: AbstractObjectDetectionService,
  ) {
    super()
    this.$s.add(this.store.select(ImageState.image).subscribe(i => (this.image = i as any)))
    this.$s.add(this.store.select(ObjectDetectionState.labels).subscribe(l => (this.labels = l)))
    this.$s.add(this.store.select(TopicState.selectedTopic).subscribe(t => (this.topic = t as any)))
  }

  save() {
    this.objectDetectionService
      .saveOrUpdate(this.topic, this.image.Id, this.labels)
      .subscribe(
        () => this.notificationService.success('Labels saved'),
        e => this.notificationService.error('Cannot Save labels', e),
      )
  }
}
