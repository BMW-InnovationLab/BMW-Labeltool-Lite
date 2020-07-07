import { ModuleWithProviders, NgModule } from '@angular/core'

import { AbstractRcvObjectclassRepository } from './abstract.rcv-objectclass.repository'
import { HttpRcvObjectclassRepository } from './http.rcv-objectclass.repository'
import { MockRcvObjectclassRepository } from './mock.rcv-objectclass.repository'

@NgModule({
  providers: [],
})
export class RcvObjectclassModule {
  static forRoot(mock: boolean): ModuleWithProviders {
    return {
      providers: [
        {
          provide: AbstractRcvObjectclassRepository,
          useClass: mock ? MockRcvObjectclassRepository : HttpRcvObjectclassRepository,
        },
      ],
      ngModule: RcvObjectclassModule,
    }
  }
}
