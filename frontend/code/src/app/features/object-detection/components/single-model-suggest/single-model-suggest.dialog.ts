import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core'
import { FormControl, FormGroup, Validators } from '@angular/forms'
import { MatDialog, MatDialogRef } from '@angular/material/dialog'

import { ConfirmModalComponent, DialogParameter } from '@common'
import { IService } from '@domain/object-detection'
import { TranslateService } from '@ngx-translate/core'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { filter } from 'rxjs/operators'
import { ModelSuggestFormComponent } from '../../components/model-suggest-form/model-suggest-form.component'

@Component({
  selector: 'rcv-single-model-suggest',
  templateUrl: './single-model-suggest.dialog.html',
  styleUrls: ['./single-model-suggest.dialog.scss'],
})
export class SingleModelSuggestComponent implements OnInit {
  @ViewChild('suggestForm', { static: true }) suggestForm: ModelSuggestFormComponent

  @Input() form: FormGroup
  @Input() objectClasses: RcvObjectClassViewInterface[] = []
  @Input() services: IService[] = []

  @Output() suggestSingleModel: EventEmitter<any> = new EventEmitter()

  @Output() expandModels = new EventEmitter<IService>()

  singleForm: FormGroup

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<SingleModelSuggestComponent>,
    private translate: TranslateService,
  ) {}

  ngOnInit(): void {
    this.setForm()
  }

  setForm() {
    this.singleForm = new FormGroup({
      imageCount: new FormControl(1, Validators.required),
    })
  }

  submit(form: FormGroup) {
    const suggestForm = this.suggestForm.submit()
    if (form.invalid || !suggestForm) {
      return
    }

    if (form.value.imageCount === 1) {
      this.doSubmit(suggestForm, form)
      return
    }

    const dialog = this.dialog.open<ConfirmModalComponent, DialogParameter>(ConfirmModalComponent, {
      data: {
        confirmTitle: this.translate.instant('Really run single model suggest?'),
        confirmText: '',
        acknowledgeText: this.translate.instant('Run'),
      },
    })
    dialog
      .afterClosed()
      .pipe(filter(r => !!r))
      .subscribe(() => this.doSubmit(suggestForm, form))
  }

  private doSubmit(suggestForm: FormGroup, form: FormGroup) {
    this.suggestSingleModel.emit({
      suggestion: suggestForm.value,
      times: form.value.imageCount,
    })
    this.dialogRef.close(suggestForm)
  }
}
