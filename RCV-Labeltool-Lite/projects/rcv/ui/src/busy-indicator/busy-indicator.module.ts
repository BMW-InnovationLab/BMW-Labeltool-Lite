import { CommonModule } from '@angular/common'
import { HTTP_INTERCEPTORS } from '@angular/common/http'
import { NgModule } from '@angular/core'
import { MatProgressBarModule } from '@angular/material'

import { BusyIndicatorInterceptor } from './busy-indicator.interceptor'
import { BusyIndicatorService } from './busy-indicator.service'
import { BusyIndicatorComponent } from './components/busy-indicator.component'

@NgModule({
  imports: [CommonModule, MatProgressBarModule],
  declarations: [BusyIndicatorComponent],
  providers: [
    BusyIndicatorService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: BusyIndicatorInterceptor,
      multi: true,
    },
  ],
  exports: [BusyIndicatorComponent],
})
export class BusyIndicatorModule {}
