import { CommonModule } from '@angular/common'
import { NgModule } from '@angular/core'
import { MatButtonModule, MatDialogModule, MatIconModule, MatSnackBarModule } from '@angular/material'

import { MessageBoxComponent } from './components/message-box/message-box.component'

@NgModule({
  imports: [CommonModule, MatSnackBarModule, MatDialogModule, MatIconModule, MatButtonModule],
  declarations: [MessageBoxComponent],
  entryComponents: [MessageBoxComponent],
})
export class NotificationModule {}
