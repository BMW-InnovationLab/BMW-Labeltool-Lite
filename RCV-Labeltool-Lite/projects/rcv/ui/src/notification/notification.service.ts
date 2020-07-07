import { Injectable } from '@angular/core'
import { MatDialog, MatSnackBar, MatSnackBarRef, SimpleSnackBar } from '@angular/material'

import { MessageBoxComponent } from './components/message-box/message-box.component'
import { ButtonLabel } from './models/button-label.enum'
import { MessageBoxActionButton } from './models/message-box-action-button.interface'

const defaultTimeout = 3000
const defaultErrorTimeout = 10000

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private snackBar: MatSnackBar, public dialog: MatDialog) {}

  error(message: string, error?: any): MatSnackBarRef<SimpleSnackBar> {
    return this.snackBar.open(message, undefined, {
      duration: defaultErrorTimeout,
      panelClass: 'notification-error',
    })
  }

  success(message: string): MatSnackBarRef<SimpleSnackBar> {
    return this.snackBar.open(message, undefined, { duration: defaultTimeout })
  }

  confirm(
    header: string,
    message: string,
    acknowledgeLabel: ButtonLabel | string = ButtonLabel.OK,
    warning: boolean = false,
    cancelLabel: ButtonLabel | string = ButtonLabel.CANCEL,
  ): Promise<boolean> {
    const actions: MessageBoxActionButton[] = [
      { label: cancelLabel, result: false },
      { label: acknowledgeLabel, result: true, warning: warning },
    ]
    return this.message(header, message, actions)
  }

  message(header: string, message: string, buttons: MessageBoxActionButton[]): Promise<any> {
    return this.dialog
      .open(MessageBoxComponent, {
        data: {
          header: header,
          message: message ? message : '',
          buttons: buttons,
        },
        autoFocus: false,
        disableClose: true,
        hasBackdrop: true,
        maxWidth: 500,
      })
      .afterClosed()
      .toPromise()
  }
}
