import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core'

import { ILabel } from '@domain/object-detection'
import { RcvObjectclassInterface } from '@rcv/domain'

@Component({
  selector: 'rcv-label-list',
  templateUrl: './label-list.component.html',
  styleUrls: ['./label-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LabelListComponent {
  @Input() objectClass: RcvObjectclassInterface
  @Input() labels: ILabel[] = []
  @Input() selected: ILabel
  @Input() hovered: ILabel

  @Output() select: EventEmitter<ILabel> = new EventEmitter()
  @Output() hover: EventEmitter<ILabel> = new EventEmitter()
  @Output() remove: EventEmitter<ILabel> = new EventEmitter()
  @Output() toggle: EventEmitter<ILabel> = new EventEmitter()

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
