import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core'
import { FormControl, Validators } from '@angular/forms'
import { MatSelectChange } from '@angular/material/select'

import { digitFromInput } from '@core/libraries/input'
import { ILabel } from '@domain/object-detection'
import { TranslateService } from '@ngx-translate/core'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'

@Component({
  selector: 'rcv-selected-label-options',
  templateUrl: './selected-label-options.component.html',
  styleUrls: ['./selected-label-options.component.scss'],
})
export class SelectedLabelOptionsComponent {
  @Input() objectClasses: RcvObjectClassViewInterface[]
  @Input()
  set selectedLabel(value: ILabel) {
    this._selectedLabel = value
    this.trackId.setValue(null)
    this.distance.setValue(null)

    if (value) {
      this.trackId.setValue(value.TrackId)
      let distance = null
      if (value.Distance != null) {
        distance = (value.Distance / 1000).toString()
        if (this.getDecimalSeparator() === ',') {
          distance = distance.replace('.', ',')
        }
      }

      this.distance.setValue(distance)
      // select object class from current label
      this.selectObjectClass.emit(this.objectClasses.find(o => o.Id === value.ObjectClassId))
    }
  }
  get selectedLabel() {
    return this._selectedLabel
  }
  @Input() enableTrackId = false
  @Input() enableDistance = false

  @Output() selectObjectClass: EventEmitter<RcvObjectClassViewInterface> = new EventEmitter()
  @Output() updateLabel: EventEmitter<ILabel> = new EventEmitter()

  trackId = new FormControl(null, [Validators.pattern(/^\d{0,10}$/)])
  distance = new FormControl(null, [Validators.min(0), Validators.max(999999.999)])

  private _selectedLabel: ILabel

  constructor(private translateService: TranslateService) {}

  @HostListener('keydown', ['$event'])
  keyDown(event: KeyboardEvent) {
    // catch all keydown events in this component
    event.stopPropagation()
  }

  @HostListener('keyup', ['$event'])
  keyUp(event: KeyboardEvent) {
    // catch all keydown events in this component
    event.stopPropagation()
  }

  @HostListener('window:keyup', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (!event.shiftKey) {
      return
    }

    // select objectclass shortcut
    const { valid, digit } = digitFromInput(event)
    if (valid) {
      const index = digit - 1
      if (index < this.objectClasses.length) {
        this.changeObjectClassOfLabel({ value: this.objectClasses[index].Id } as any)
      }
    }
  }

  changeObjectClassOfLabel(event: MatSelectChange) {
    const objectClass = this.objectClasses.find(o => o.Id === event.value)
    if (!objectClass) {
      return
    }

    this.selectObjectClass.emit(objectClass)
    this.updateLabel.emit({
      ...this.selectedLabel,
      ObjectClassId: objectClass.Id != null ? objectClass.Id : -1,
      ObjectClassName: objectClass.Name,
    })
  }

  changeTrackId() {
    if (this.trackId.valid) {
      const value = this.trackId.value
      this.updateLabel.emit({ ...this.selectedLabel, TrackId: value === '' ? null : value })
    }
  }

  changeDistance(value: any) {
    if (this.distance.valid) {
      this.updateLabel.emit({
        ...this.selectedLabel,
        Distance: value === '' ? null : Math.round(parseFloat(value.replace(',', '.')) * 1000),
      })
    }
  }

  getDecimalSeparator() {
    return this.translateService.currentLang === 'de' ? ',' : '.'
  }
}
