import { NgModule } from '@angular/core'
import { NgxsModule } from '@ngxs/store'

import { ObjectDetectionService } from './services/object-detection.service'
import { ObjectDetectionState } from './store/object-detection.state'

@NgModule({
  imports: [NgxsModule.forFeature([ObjectDetectionState])],
  providers: [
    {
      provide: 'IObjectDetectionService',
      useClass: ObjectDetectionService,
    },
  ],
})
export class ObjectDetectionDomainModule {}
