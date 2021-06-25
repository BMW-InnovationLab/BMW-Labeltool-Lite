import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { MatSelectChange } from '@angular/material/select'

import { selectCompare } from '@core/libraries/entity'
import { IService } from '@domain/object-detection'
import { RcvModelInterface, RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { BehaviorSubject, EMPTY, Observable } from 'rxjs'
import { map } from 'rxjs/operators'

@Component({
  selector: 'rcv-model-suggest-form',
  templateUrl: './model-suggest-form.component.html',
  styleUrls: ['./model-suggest-form.component.scss'],
})
export class ModelSuggestFormComponent implements OnInit {
  @Input() form: FormGroup
  @Input() objectClasses: RcvObjectClassViewInterface[]

  objectDetectionServices$: Observable<IService[]> = EMPTY

  private services$ = new BehaviorSubject<IService[]>([])
  @Input() set services(value: IService[]) {
    this.services$.next(value)
  }

  @Output() expandModels = new EventEmitter<IService>()

  get objectDetectionModels(): RcvModelInterface[] {
    if (!this.form) {
      return []
    }

    const service: IService = this.form.value.service
    return service && service.objectDetectionModels ? service.objectDetectionModels : []
  }

  get detectsObjectClasses(): boolean {
    const service: IService = this.form.controls['service'].value
    return service != null ? service.DetectObjectClasses : true
  }

  selectCompare = selectCompare

  constructor() {}

  ngOnInit(): void {
    if (!this.form) {
      this.setForm()
    } else {
      // setup object detection
      this.changeService({ value: this.form.value.service } as any)
    }

    this.objectDetectionServices$ = this.services$.pipe(
      map(services => services.filter(service => service.SupportsObjectDetection)),
    )
  }

  setForm() {
    this.form = new FormGroup({
      service: new FormControl(undefined, Validators.required),
      model: new FormControl(undefined, Validators.required),
      objectClass: new FormControl(),
    })
  }

  changeService(event: MatSelectChange): void {
    const service: IService = event.value
    if (!service) {
      return
    }

    this.form.setControl(
      'objectClass',
      this.buildControl(service.DetectObjectClasses, this.form.value.objectClass),
    )

    this.expandModels.emit(service)
  }

  submit(): FormGroup | undefined {
    // show error hints
    this.form.controls['service'].markAsTouched()
    this.form.controls['model'].markAsTouched()
    this.form.controls['objectClass'].markAsTouched()

    if (this.form.invalid) {
      return undefined
    }

    return this.form
  }

  private buildControl(optional: boolean, value?: any) {
    return new FormControl(value, optional ? [] : Validators.required)
  }
}
