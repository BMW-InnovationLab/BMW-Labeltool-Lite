import { Directive, HostListener } from '@angular/core'
import { DefaultValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms'

@Directive({
  /* tslint:disable:directive-selector */
  selector: 'input[trim], textarea[trim]',
  providers: [{ provide: NG_VALUE_ACCESSOR, useExisting: TrimDirective, multi: true }],
})
export class TrimDirective extends DefaultValueAccessor {
  @HostListener('blur', ['$event.target.value'])
  onBlur(value: string) {
    this.processValue(value)
    this.onTouched()
  }

  private processValue(value: any) {
    if (typeof value === 'string') {
      value = value.trim()
    }

    this.writeValue(value)
    this.onChange(value)
  }
}
