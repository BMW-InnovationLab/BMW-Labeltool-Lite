import { NgModule } from '@angular/core'

import { TrimDirective } from './trim.directive'

@NgModule({
  declarations: [TrimDirective],
  exports: [TrimDirective],
})
export class InputTrimModule {}
