import { ModuleWithProviders, NgModule } from '@angular/core'

import { AbstractRcvTopicRepository } from './repository/abstract.rcv-topic.repository'
import { HttpRcvTopicRepository } from './repository/http.rcv-topic.repository'
import { MockRcvTopicRepository } from './repository/mock.rcv-topic.repository'

@NgModule({
  providers: [],
})
export class RcvTopicModule {
  static forRoot(mock: boolean): ModuleWithProviders {
    return {
      providers: [
        {
          provide: AbstractRcvTopicRepository,
          useClass: mock ? MockRcvTopicRepository : HttpRcvTopicRepository,
        },
      ],
      ngModule: RcvTopicModule,
    }
  }
}
