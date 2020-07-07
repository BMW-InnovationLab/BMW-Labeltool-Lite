import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { MatSelectChange } from '@angular/material'

import { ILabel } from '@domain/object-detection'
import { RcvObjectclassInterface } from '@rcv/domain'

@Component({
  selector: 'rcv-selected-label-options',
  templateUrl: './selected-label-options.component.html',
  styleUrls: ['./selected-label-options.component.scss'],
})
export class SelectedLabelOptionsComponent implements OnInit {
  @Input() objectClasses: RcvObjectclassInterface[]
  @Input()
  set selectedLabel(value: ILabel) {
    this._selectedLabel = value
    if (value) {
      // select object class from current label
      this.selectObjectClass.emit(this.objectClasses.find(o => o.Id === value.ObjectClassId))
    }
  }
  get selectedLabel() {
    return this._selectedLabel
  }

  @Output() selectObjectClass: EventEmitter<RcvObjectclassInterface> = new EventEmitter()
  @Output() updateLabel: EventEmitter<ILabel> = new EventEmitter()

  private _selectedLabel: ILabel

  constructor() {}

  ngOnInit() {}

  changeObjectClassOfLabel(event: MatSelectChange) {
    const objectClass = this.objectClasses.find(o => o.Id === event.value)
    if (!objectClass) {
      return
    }

    this.selectObjectClass.emit(objectClass)
    this.updateLabel.emit({
      ...this.selectedLabel,
      ObjectClassId: objectClass.Id != null ? objectClass.Id : '',
      ObjectClassName: objectClass.Name,
    })
  }
}
