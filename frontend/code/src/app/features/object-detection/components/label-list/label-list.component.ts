import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core'

import { ILabel } from '@domain/object-detection'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'

@Component({
  selector: 'rcv-label-list',
  templateUrl: './label-list.component.html',
  styleUrls: ['./label-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LabelListComponent {
  @Input() objectClass: RcvObjectClassViewInterface
  @Input() labels: ILabel[] = []
  @Input() selected: ILabel
  @Input() hovered: ILabel

  @Output() copy = new EventEmitter<ILabel>()
  @Output() select = new EventEmitter<ILabel>()
  @Output() hover = new EventEmitter<ILabel>()
  @Output() remove = new EventEmitter<ILabel>()
  @Output() toggle = new EventEmitter<ILabel>()

  private expanded = false

  expandPanel(): boolean {
    // if already expanded don't touch it
    if (this.expanded) {
      return true
    }

    if (!this.selected) {
      return false
    }
    this.expanded = this.labels.some(l => l.Id === this.selected.Id)
    return this.expanded
  }

  trackLabels(index: number, label: ILabel) {
    return label.Id
  }
}
