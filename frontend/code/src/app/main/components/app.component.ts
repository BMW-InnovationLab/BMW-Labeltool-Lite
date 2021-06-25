import { Component, OnInit } from '@angular/core'
import { MatIconRegistry } from '@angular/material/icon'
import { DomSanitizer } from '@angular/platform-browser'
import { TranslateService } from '@ngx-translate/core'
import { Actions, ofActionDispatched } from '@ngxs/store'

import { SuccessNotification } from '@domain/notification'
import { NotificationService } from '@rcv/ui'
import enLocale from 'src/assets/i18n/en.json'

@Component({
  selector: 'rcv-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(
    private actions$: Actions,
    private iconRegistry: MatIconRegistry,
    private sanitizer: DomSanitizer,
    private translate: TranslateService,
    private notification: NotificationService,
  ) {
    this.addIcons()
  }

  ngOnInit() {
    this.translate.setTranslation('en', enLocale, true)
    this.translate.use('en')
    this.translate.resetLang('de')

    this.handleNotifications()
  }

  private handleNotifications() {
    this.actions$
      .pipe(ofActionDispatched(SuccessNotification))
      .subscribe(({ message }: SuccessNotification) => this.notification.success(message))
  }

  private registerIcon(name: string, path: string) {
    this.iconRegistry.addSvgIcon(name, this.sanitizer.bypassSecurityTrustResourceUrl(path))
  }

  private addIcons() {
    this.registerIcon('add', 'assets/icons/add.svg')
    this.registerIcon('add-circle', 'assets/icons/add-circle.svg')
    this.registerIcon('arrange-bring-forward', 'assets/icons/arrange-bring-forward.svg')
    this.registerIcon('arrange-send-backward', 'assets/icons/arrange-send-backward.svg')
    this.registerIcon('arrow-down', 'assets/icons/arrow-down.svg')
    this.registerIcon('arrow-right', 'assets/icons/arrow-right.svg')
    this.registerIcon('backup-restore', 'assets/icons/backup-restore.svg')
    this.registerIcon('check', 'assets/icons/check.svg')
    this.registerIcon('chevron-double-left', 'assets/icons/chevron-double-left.svg')
    this.registerIcon('chevron-double-right', 'assets/icons/chevron-double-right.svg')
    this.registerIcon('chevron-left', 'assets/icons/chevron-left.svg')
    this.registerIcon('chevron-right', 'assets/icons/chevron-right.svg')
    this.registerIcon('close', 'assets/icons/close.svg')
    this.registerIcon('collections', 'assets/icons/collections.svg')
    this.registerIcon('content-save', 'assets/icons/content-save.svg')
    this.registerIcon('copy', 'assets/icons/copy.svg')
    this.registerIcon('delete', 'assets/icons/delete.svg')
    this.registerIcon('eraser', 'assets/icons/eraser.svg')
    this.registerIcon('expand-less', 'assets/icons/expand-less.svg')
    this.registerIcon('expand-more', 'assets/icons/expand-more.svg')
    this.registerIcon('eye', 'assets/icons/eye.svg')
    this.registerIcon('eye-off', 'assets/icons/eye-off.svg')
    this.registerIcon('help-circle', 'assets/icons/help-circle.svg')
    this.registerIcon('more-vert', 'assets/icons/more-vert.svg')
    this.registerIcon('pen', 'assets/icons/pen.svg')
    this.registerIcon('remove', 'assets/icons/remove.svg')
    this.registerIcon('info', 'assets/icons/info.svg')
    this.registerIcon('upload', 'assets/icons/upload.svg')
    this.registerIcon('vector-rectangle', 'assets/icons/vector-rectangle.svg')
  }
}
