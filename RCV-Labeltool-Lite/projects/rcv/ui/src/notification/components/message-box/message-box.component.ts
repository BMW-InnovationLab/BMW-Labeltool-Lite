import { Component, Inject, OnInit } from '@angular/core'
import { MAT_DIALOG_DATA } from '@angular/material'

import { ButtonLabel } from '../../models/button-label.enum'
import { MessageBoxActionButton } from '../../models/message-box-action-button.interface'
import { MessageBoxParamsInterface } from '../../models/message-box-params.interface'

@Component({
  selector: 'rcv-message-box',
  templateUrl: './message-box.component.html',
  styleUrls: ['./message-box.component.scss'],
})
export class MessageBoxComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: MessageBoxParamsInterface) {}

  ngOnInit() {}

  get Header(): string {
    return this.data.header
  }

  get Message(): string {
    return this.data.message
  }

  get Actions(): MessageBoxActionButton[] {
    return this.data.buttons
  }

  buttonLabel(label: ButtonLabel | string): string {
    if (typeof label === 'string') {
      return 'label'
    } else {
      switch (label) {
        case ButtonLabel.CANCEL:
          return 'Cancel'
        case ButtonLabel.DELETE:
          return 'Delete'
        case ButtonLabel.OK:
          return 'Ok'
      }
    }

    return 'Ok'
  }
}
