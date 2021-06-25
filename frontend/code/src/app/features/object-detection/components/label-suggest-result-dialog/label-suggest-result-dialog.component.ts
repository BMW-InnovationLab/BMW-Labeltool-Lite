import { Component, Inject } from '@angular/core'
import { MAT_DIALOG_DATA } from '@angular/material/dialog'
import { Store } from '@ngxs/store'
import { Observable } from 'rxjs'
import { map } from 'rxjs/operators'

import { MultiSuggestionResult, SingleSuggestionResult } from '@domain/object-detection'
import { TopicState } from '@domain/topic'

export type DialogParameter = SingleSuggestionResult | MultiSuggestionResult

@Component({
  selector: 'rcv-label-suggest-result-dialog',
  templateUrl: './label-suggest-result-dialog.component.html',
  styleUrls: ['./label-suggest-result-dialog.component.scss'],
})
export class LabelSuggestResultDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogParameter, private store: Store) {}

  objectClassName(id: number): Observable<string> {
    return this.store
      .select(TopicState.objectclass(id))
      .pipe(map(objectClass => (objectClass ? objectClass.Name : '')))
  }
}
