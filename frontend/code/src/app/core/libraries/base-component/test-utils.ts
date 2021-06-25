import { TestBed } from '@angular/core/testing'
import { RouterTestingModule } from '@angular/router/testing'
import { TranslateModule } from '@ngx-translate/core'
import { NgxsModule } from '@ngxs/store'

import { ImageState } from '@domain/image'
import { TopicState } from '@domain/topic'
import { AbstractRcvImageRepository, MockRcvImageRepository } from '@rcv/domain'
import { NotificationService } from '@rcv/ui'
import { AppInjector } from './app-injector'

export function setupTestInjector() {
  TestBed.configureTestingModule({
    imports: [
      RouterTestingModule.withRoutes([]),
      TranslateModule.forRoot(),
      NgxsModule.forRoot([ImageState, TopicState]),
    ],
    providers: [
      {
        provide: NotificationService,
        useValue: { error: jest.fn(), confirm: jest.fn(), success: jest.fn() },
      },
      { provide: AbstractRcvImageRepository, useClass: MockRcvImageRepository },
    ],
  })
  AppInjector.setInjector(TestBed)
}
