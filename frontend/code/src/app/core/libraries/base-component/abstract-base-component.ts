import { OnDestroy } from '@angular/core'
import { TranslateService } from '@ngx-translate/core'
import { Store } from '@ngxs/store'
import { Subscription } from 'rxjs'

import { NotificationService } from '@rcv/ui'
import { AppInjector } from './app-injector'

export abstract class AbstractBaseComponent implements OnDestroy {
  protected translate: TranslateService
  protected store: Store
  protected notificationService: NotificationService

  protected readonly $s: Subscription = new Subscription()

  constructor() {
    const injector = AppInjector.getInjector()
    this.translate = injector.get(TranslateService)
    this.store = injector.get(Store)
    this.notificationService = injector.get(NotificationService)
  }

  ngOnDestroy(): void {
    this.$s.unsubscribe()
  }
}
