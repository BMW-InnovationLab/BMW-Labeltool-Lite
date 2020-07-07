import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'

@Component({
  selector: 'rcv-save-actions',
  templateUrl: './save-actions.component.html',
  styleUrls: ['./save-actions.component.scss'],
})
export class SaveActionsComponent implements OnInit {
  @Input() disabled = false

  @Output() save: EventEmitter<void> = new EventEmitter()

  constructor() {}

  ngOnInit() {}
}
