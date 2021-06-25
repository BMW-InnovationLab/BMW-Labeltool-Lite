import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { RcvLabelMode } from '@rcv/domain'
import { ImagePageComponent } from './container/image-page/image-page.component'
import { objectDetectionPath } from './label-mode-routing'

export const routes: Routes = [
  {
    path: 'topics/:topicId',
    component: ImagePageComponent,
    children: [
      {
        path: objectDetectionPath,
        loadChildren: () =>
          import('src/app/features/object-detection/object-detection.module').then(
            m => m.ObjectDetectionModule,
          ),
        pathMatch: 'full',
        data: { mode: RcvLabelMode.ObjectDetection },
      },
      {
        path: '**',
        redirectTo: objectDetectionPath,
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
