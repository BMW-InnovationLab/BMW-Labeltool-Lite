import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { RcvLabelMode } from '@rcv/domain'
import { ImagePageComponent } from './container/image-page/image-page.component'

export const routes: Routes = [
  {
    path: 'topics/:topicId',
    component: ImagePageComponent,
    children: [
      {
        path: 'object-detection',
        loadChildren: 'src/app/features/object-detection/object-detection.module#ObjectDetectionModule',
        pathMatch: 'full',
        data: { mode: RcvLabelMode.ObjectDetection },
      },
      {
        path: '**',
        redirectTo: 'object-detection',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'topics/0',
    pathMatch: 'full',
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ImageRoutingModule {}
