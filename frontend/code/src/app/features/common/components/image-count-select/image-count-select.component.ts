import { Component, forwardRef, OnInit } from '@angular/core'
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms'

@Component({
  selector: 'rcv-image-count-select',
  templateUrl: './image-count-select.component.html',
  styleUrls: ['./image-count-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ImageCountSelectComponent),
      multi: true,
    },
  ],
})
export class ImageCountSelectComponent implements ControlValueAccessor, OnInit {
  counts = [0, 1, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000]
  private _value: number

  get value() {
    return this._value
  }
  set value(value: number) {
    this._value = value
    this.propagateChange(value)
  }

  constructor() {}

  ngOnInit() {}

  propagateChange = (_: any) => {}

  writeValue(value: any): void {
    this.value = value
  }

  registerOnChange(fn: any) {
    this.propagateChange = fn
  }

  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}
}
