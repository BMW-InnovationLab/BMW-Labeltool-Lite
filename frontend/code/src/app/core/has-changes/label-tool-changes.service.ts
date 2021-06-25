import { Injectable } from '@angular/core'
import { SetChanges, TopicState } from '@domain/topic'
import { Select, Store } from '@ngxs/store'
import { ChangesService, NotificationService } from '@rcv/ui'
import { Observable } from 'rxjs'
import { first } from 'rxjs/operators'

@Injectable({ providedIn: 'root' })
export class LabelToolChangesService extends ChangesService {
  @Select(TopicState.hasChanges) hasChanges$: Observable<boolean>

  constructor(private store: Store, notificationService: NotificationService) {
    super(notificationService)
  }

  registerListeners() {
    this.registerListener(
      async () => await this.hasChanges$.pipe(first()).toPromise(),
      () => this.store.dispatch(new SetChanges(false)),
    )
  }

  async hasChanges() {
    return !(await this.checkHasChanges())
  }
}
