import { Component, EventEmitter, Input, Output } from '@angular/core'

import { ILabel } from '@domain/object-detection'

@Component({
  selector: 'rcv-label-list-item',
  templateUrl: './label-list-item.component.html',
  styleUrls: ['./label-list-item.component.scss'],
})
export class LabelListItemComponent {
  @Input() label: ILabel

  @Output() toggle: EventEmitter<ILabel> = new EventEmitter()
  @Output() remove: EventEmitter<ILabel> = new EventEmitter()

  constructor() {}
}
