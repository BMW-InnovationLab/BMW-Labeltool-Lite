import { NgModule } from '@angular/core'
import { NgxsModule } from '@ngxs/store'

import { environment } from '@environment'
import { ObjectDetectionService } from './services/object-detection.service'
import { ObjectDetectionServiceMock } from './services/object-detection.service.mock'
import { ObjectDetectionState } from './store/object-detection.state'

@NgModule({
  imports: [NgxsModule.forFeature([ObjectDetectionState])],
  providers: [
    {
      provide: 'IObjectDetectionService',
      useClass: environment.services.mock ? ObjectDetectionServiceMock : ObjectDetectionService,
    },
  ],
})
export class ObjectDetectionDomainModule {}
