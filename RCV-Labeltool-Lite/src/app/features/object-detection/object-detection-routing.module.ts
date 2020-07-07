import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { LabelPageComponent } from './container/label-page/label-page.component'
import { LabelSaveActionsComponent } from './container/label-save-actions/label-save-actions.component'
import { LabelSideNavComponent } from './container/label-side-nav/label-side-nav.component'

const routes: Routes = [
  {
    path: '',
    component: LabelPageComponent,
  },
  {
    path: '',
    component: LabelSideNavComponent,
    outlet: 'list',
  },
  {
    path: '',
    component: LabelSaveActionsComponent,
    outlet: 'save-actions',
  },
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full',
  },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ObjectDetectionRoutingModule {}
