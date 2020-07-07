import { Component } from '@angular/core'
import { MatDialog } from '@angular/material'
import { Select } from '@ngxs/store'
import { Observable } from 'rxjs'
import { combineLatest, filter } from 'rxjs/operators'

import { AbstractBaseComponent } from '@core'
import { ImageState, LoadInitialImage } from '@domain/image'
import { SelectTopic, TopicState } from '@domain/topic'
import { RcvImageInterface, RcvLabelMode, RcvTopicInterface } from '@rcv/domain'
import { HelpDialogComponent } from '../../components/help-dialog/help-dialog.component'
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
  @Select(ImageState.mode) mode$: Observable<RcvLabelMode>
  @Select(TopicState.selectedTopic) selectedTopic$: Observable<RcvTopicInterface>

  image: RcvImageInterface
  topic: RcvTopicInterface
  topics: RcvTopicInterface[] = []

  constructor(private dialog: MatDialog) {
    super()
    this.$s.add(this.store.select(ImageState.image).subscribe(i => (this.image = i as any)))
    this.$s.add(this.store.select(TopicState.topics).subscribe(t => (this.topics = t)))
    // trigger reloads if label mode changes
    this.$s.add(
      this.selectedTopic$.pipe(combineLatest(this.mode$)).subscribe(v => this.handleTopicChange(v[0])),
    )
  }

  selectTopic() {
    this.dialog
      .open<TopicSelectDialogComponent, TopicDialogParameter>(TopicSelectDialogComponent, {
        width: '600px',
        data: { selectedTopic: this.topic, topics: this.topics },
      })
      .afterClosed()
      .pipe(filter(r => !!r))
      .subscribe(topic => this.store.dispatch([new SelectTopic(topic)]))
  }

  showHelp(): void {
    this.dialog.open(HelpDialogComponent, {
      maxHeight: '100vh',
    })
  }

  private handleTopicChange(topic: RcvTopicInterface) {
    if (!!topic) {
      this.topic = topic
      this.store.dispatch([new LoadInitialImage(topic)])
    }
  }
}
