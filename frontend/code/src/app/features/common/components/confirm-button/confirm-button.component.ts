import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'

@Component({
  selector: 'rcv-confirm-button',
  templateUrl: './confirm-button.component.html',
  styleUrls: ['./confirm-button.component.scss'],
})
export class ConfirmButtonComponent implements OnInit {
  @Input() confirmText = 'Want to remove?'
  @Input() disabled = false
  @Input() tooltip = 'ACTIONS.REMOVE'

  @Output() remove: EventEmitter<any> = new EventEmitter()

  sure = false

  constructor() {}

  ngOnInit() {}

  toggleSure(event: MouseEvent) {
    this.sure = !this.sure
    event.stopPropagation()
  }

  emitRemove() {
    this.remove.emit()
    this.sure = false
  }
}
