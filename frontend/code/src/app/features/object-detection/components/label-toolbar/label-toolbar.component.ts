import { ChangeDetectionStrategy, Component, EventEmitter, HostListener, Input, Output } from '@angular/core'

import { ILabel } from '@domain/object-detection'
import { RcvImageLabelInterface } from '@rcv/domain/labeltool-client'

@Component({
  selector: 'rcv-label-toolbar',
  templateUrl: './label-toolbar.component.html',
  styleUrls: ['./label-toolbar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LabelToolbarComponent {
  @Input() image: RcvImageLabelInterface
  @Input() labels: ILabel[]
  @Input() visible: boolean

  @Output() toggle: EventEmitter<boolean> = new EventEmitter()
  @Output() restore: EventEmitter<any> = new EventEmitter()
  @Output() remove: EventEmitter<any> = new EventEmitter()

  @HostListener('window:keyup', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (!event.shiftKey) {
      return
    }
    switch (event.key) {
      case 'D':
        this.remove.emit()
        break
      case 'A':
        this.restore.emit()
        break

      default:
      /* do nothing */
    }
  }
}
