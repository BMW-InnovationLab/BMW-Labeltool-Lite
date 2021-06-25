import { Component, Inject, OnInit } from '@angular/core'
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog'

export interface DialogParameter {
  confirmTitle: string
  confirmText: string
  acknowledgeText: string
  isDangerous?: boolean
}

@Component({
  selector: 'rcv-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.scss'],
})
export class ConfirmModalComponent implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<ConfirmModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogParameter,
  ) {}

  ngOnInit() {}
}
