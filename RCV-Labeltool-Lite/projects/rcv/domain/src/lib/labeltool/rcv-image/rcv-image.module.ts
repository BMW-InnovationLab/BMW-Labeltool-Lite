import { ModuleWithProviders, NgModule } from '@angular/core'

import { AbstractRcvImageRepository } from './repository/abstract.rcv-image.repository'
import { HttpRcvImageRepository } from './repository/http.rcv-image.repository'
import { MockRcvImageRepository } from './repository/mock.rcv-image.repository'

@NgModule({
  declarations: [],
  providers: [],
})
export class RcvImageModule {
  static forRoot(mock: boolean): ModuleWithProviders {
    return {
      providers: [
        {
          provide: AbstractRcvImageRepository,
          useClass: mock ? MockRcvImageRepository : HttpRcvImageRepository,
        },
      ],
      ngModule: RcvImageModule,
    }
  }
}
