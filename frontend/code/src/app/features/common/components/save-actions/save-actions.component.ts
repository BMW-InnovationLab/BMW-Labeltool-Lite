import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core'

@Component({
  selector: 'rcv-save-actions',
  templateUrl: './save-actions.component.html',
  styleUrls: ['./save-actions.component.scss'],
})
export class SaveActionsComponent implements OnInit {
  @Input() disabled = false

  @Input()
  saveLabelKey = 'ACTIONS.SAVE'

  @Input()
  saveAndNextLabelKey = 'ACTIONS.SAVE_AND_NEXT'

  @Output() save: EventEmitter<void> = new EventEmitter()
  @Output() saveAndNext: EventEmitter<void> = new EventEmitter()

  constructor() {}

  @HostListener('window:keyup', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (!event.shiftKey) {
      return
    }
    switch (event.key) {
      case 'C':
        this.save.emit()
        break
      case 'S':
        this.saveAndNext.emit()
        break
      default:
      /* do nothing */
    }
  }

  ngOnInit() {}
}
