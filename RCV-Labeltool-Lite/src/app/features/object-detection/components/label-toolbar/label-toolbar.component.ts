import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core'

import { ILabel } from '@domain/object-detection'
import { RcvImageInterface } from '@rcv/domain'

@Component({
  selector: 'rcv-label-toolbar',
  templateUrl: './label-toolbar.component.html',
  styleUrls: ['./label-toolbar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LabelToolbarComponent {
  @Input() image: RcvImageInterface
  @Input() labels: ILabel[]
  @Input() visible: boolean

  @Output() toggle: EventEmitter<boolean> = new EventEmitter()
  @Output() restore: EventEmitter<any> = new EventEmitter()
  @Output() remove: EventEmitter<any> = new EventEmitter()
}
