import { Component, HostListener } from '@angular/core'

import { environment } from '@environment'

@Component({
  templateUrl: './help-dialog.component.html',
  styleUrls: ['./help-dialog.component.scss'],
})
export class HelpDialogComponent {
  version = environment.version

  objectClassShortcuts = [...Array(9).keys()]

  @HostListener('keyup', ['$event'])
  keyup(event: KeyboardEvent) {
    // don't bubble shortcuts up
    event.stopPropagation()
  }
}
