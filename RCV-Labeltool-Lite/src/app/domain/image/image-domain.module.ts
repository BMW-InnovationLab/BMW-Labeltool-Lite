import { NgModule } from '@angular/core'
import { NgxsModule } from '@ngxs/store'

import { ImageState } from './store/image.state'

@NgModule({
  imports: [NgxsModule.forFeature([ImageState])],
})
export class ImageDomainModule {}
