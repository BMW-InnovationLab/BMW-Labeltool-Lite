import { Component, EventEmitter, Input, Output } from '@angular/core'

import { ILabel } from '@domain/object-detection'

@Component({
  selector: 'rcv-label-list-item',
  templateUrl: './label-list-item.component.html',
  styleUrls: ['./label-list-item.component.scss'],
})
export class LabelListItemComponent {
  @Input() label: ILabel

  @Output() copy = new EventEmitter<ILabel>()
  @Output() toggle = new EventEmitter<ILabel>()
  @Output() remove = new EventEmitter<ILabel>()

  constructor() {}

  copyLabel(event: MouseEvent, label: ILabel) {
    event.stopPropagation()
    this.copy.emit(label)
  }
}
