import { CommonModule as AngularCommonModule } from '@angular/common'
import { NgModule } from '@angular/core'

import { CommonModule } from '@common'
import { BoundingBoxEditorModule } from '@svg-editor'
import { ObjectDetectionRoutingModule } from './object-detection-routing.module'

import { ObjectDetectionDomainModule } from '@domain/object-detection'
import { ImageCountSelectComponent } from './components/image-count-select/image-count-select.component'
import { LabelListItemComponent } from './components/label-list-item/label-list-item.component'
import { LabelListComponent } from './components/label-list/label-list.component'
import { LabelToolbarComponent } from './components/label-toolbar/label-toolbar.component'
import { SelectedLabelOptionsComponent } from './components/selected-label-options/selected-label-options.component'
import { LabelPageComponent } from './container/label-page/label-page.component'
import { LabelSaveActionsComponent } from './container/label-save-actions/label-save-actions.component'
import { LabelSideNavComponent } from './container/label-side-nav/label-side-nav.component'

@NgModule({
  imports: [
    AngularCommonModule,
    CommonModule,

    // store
    ObjectDetectionDomainModule,

    // components
    BoundingBoxEditorModule,

    // routing
    ObjectDetectionRoutingModule,
  ],
  declarations: [
    ImageCountSelectComponent,
    LabelListComponent,
    LabelListItemComponent,
    LabelPageComponent,
    LabelSaveActionsComponent,
    LabelSideNavComponent,
    LabelToolbarComponent,
    SelectedLabelOptionsComponent,
  ],
})
export class ObjectDetectionModule {}
