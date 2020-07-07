import { Component, OnInit } from '@angular/core'
import { MatIconRegistry } from '@angular/material'
import { DomSanitizer } from '@angular/platform-browser'

@Component({
  selector: 'rcv-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private iconRegistry: MatIconRegistry, private sanitizer: DomSanitizer) {
    this.addIcons()
  }

  ngOnInit() {}

  private registerIcon(name: string, path: string) {
    this.iconRegistry.addSvgIcon(name, this.sanitizer.bypassSecurityTrustResourceUrl(path))
  }

  private addIcons() {
    this.registerIcon('add-circle', 'assets/icons/add-circle.svg')
    this.registerIcon('add', 'assets/icons/add.svg')
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
    this.registerIcon('eye-off', 'assets/icons/eye-off.svg')
    this.registerIcon('eye', 'assets/icons/eye.svg')
    this.registerIcon('help-circle', 'assets/icons/help-circle.svg')
    this.registerIcon('info', 'assets/icons/info.svg')
    this.registerIcon('more-vert', 'assets/icons/more-vert.svg')
    this.registerIcon('pen', 'assets/icons/pen.svg')
    this.registerIcon('remove', 'assets/icons/remove.svg')
    this.registerIcon('shuffle', 'assets/icons/shuffle.svg')
    this.registerIcon('transfer', 'assets/icons/transfer.svg')
    this.registerIcon('upload', 'assets/icons/upload.svg')
    this.registerIcon('vector-rectangle', 'assets/icons/vector-rectangle.svg')
  }
}
