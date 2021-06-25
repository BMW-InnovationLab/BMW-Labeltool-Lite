import { Component, Inject } from '@angular/core'
import { MAT_DIALOG_DATA } from '@angular/material/dialog'

@Component({
  selector: 'rcv-image-data-modal',
  templateUrl: './image-data-modal.component.html',
  styleUrls: ['./image-data-modal.component.scss'],
})
export class ImageDataModalComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
}
