import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { HasChangesGuard } from '@rcv/ui'
import { LabelActionsBarComponent } from './container/label-actions-bar/label-actions-bar.component'
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
    canDeactivate: [HasChangesGuard],
    outlet: 'list',
  },
  {
    path: '',
    component: LabelActionsBarComponent,
    outlet: 'actions',
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
