import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core'
import { FormControl, FormGroup } from '@angular/forms'
import { MatDialog, MatDialogRef } from '@angular/material/dialog'

import { ConfirmModalComponent, DialogParameter } from '@common'
import { IService, ISuggestion } from '@domain/object-detection'
import { TranslateService } from '@ngx-translate/core'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { filter } from 'rxjs/operators'
import { ModelSuggestFormComponent } from '../../components/model-suggest-form/model-suggest-form.component'

@Component({
  selector: 'rcv-multi-model-suggest',
  templateUrl: './multi-model-suggest.dialog.html',
  styleUrls: ['./multi-model-suggest.dialog.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MultiModelSuggestComponent implements OnInit {
  @ViewChild('embeddedSuggestForm', { static: true }) embeddedSuggestForm: ModelSuggestFormComponent

  @Input() objectClasses: RcvObjectClassViewInterface[] = []
  @Input() services: IService[] = []

  @Output() suggestMultipleModels: EventEmitter<any> = new EventEmitter()

  @Output() expandModels = new EventEmitter<IService>()

  displayedColumns = ['Service', 'BaseModel', 'ObjectClass', 'Remove']
  dataSource: ISuggestion[] = []
  form: FormGroup
  imageCountControl = new FormControl(1)

  constructor(
    private dialog: MatDialog,
    private dialogRef: MatDialogRef<MultiModelSuggestComponent>,
    private translate: TranslateService,
  ) {}

  ngOnInit() {
    this.setForm()
  }

  setForm() {
    this.form = new FormGroup({})
  }

  addSuggest(form: FormGroup) {
    const suggestForm = this.embeddedSuggestForm.submit()
    if (!suggestForm) {
      return
    }

    this.dataSource = [
      ...this.dataSource,
      {
        ...suggestForm.value,
      },
    ]
  }

  removeSuggest(suggestion: ISuggestion) {
    this.dataSource = this.dataSource.filter(s => s !== suggestion)
  }

  suggest() {
    const dialog = this.dialog.open<ConfirmModalComponent, DialogParameter>(ConfirmModalComponent, {
      data: {
        confirmTitle: this.translate.instant('Really run multi model suggest?'),
        confirmText: '',
        acknowledgeText: this.translate.instant('Run'),
      },
    })
    dialog
      .afterClosed()
      .pipe(filter(r => !!r))
      .subscribe(r => {
        this.suggestMultipleModels.emit({
          suggestions: this.dataSource,
          times: this.imageCountControl.value,
        })
        this.dialogRef.close()
      })
  }
}
