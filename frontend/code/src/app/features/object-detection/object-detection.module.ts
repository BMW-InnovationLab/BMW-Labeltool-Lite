import { NgModule } from '@angular/core'

import { CommonModule } from '@common'
import { BoundingBoxEditorModule } from '@svg-editor'
import { DigitOnlyModule } from '@uiowa/digit-only'
import { ObjectDetectionRoutingModule } from './object-detection-routing.module'

import { ObjectDetectionDomainModule } from '@domain/object-detection'
import { LabelListItemComponent } from './components/label-list-item/label-list-item.component'
import { LabelListComponent } from './components/label-list/label-list.component'
import { LabelSuggestResultDialogComponent } from './components/label-suggest-result-dialog/label-suggest-result-dialog.component'
import { LabelToolbarComponent } from './components/label-toolbar/label-toolbar.component'
import { ModelSuggestFormComponent } from './components/model-suggest-form/model-suggest-form.component'
import { MultiModelSuggestComponent } from './components/multi-model-suggest/multi-model-suggest.dialog'
import { SelectedLabelOptionsComponent } from './components/selected-label-options/selected-label-options.component'
import { SingleModelSuggestComponent } from './components/single-model-suggest/single-model-suggest.dialog'
import { LabelActionsBarComponent } from './container/label-actions-bar/label-actions-bar.component'
import { LabelPageComponent } from './container/label-page/label-page.component'
import { LabelSaveActionsComponent } from './container/label-save-actions/label-save-actions.component'
import { LabelSideNavComponent } from './container/label-side-nav/label-side-nav.component'

@NgModule({
  imports: [
    CommonModule,

    // store
    ObjectDetectionDomainModule,

    // components
    BoundingBoxEditorModule,

    // routing
    ObjectDetectionRoutingModule,
    DigitOnlyModule,
  ],
  declarations: [
    LabelActionsBarComponent,
    LabelListComponent,
    LabelListItemComponent,
    LabelPageComponent,
    LabelSaveActionsComponent,
    LabelSideNavComponent,
    LabelSuggestResultDialogComponent,
    LabelToolbarComponent,
    ModelSuggestFormComponent,
    MultiModelSuggestComponent,
    SelectedLabelOptionsComponent,
    SingleModelSuggestComponent,
  ],
  entryComponents: [LabelSuggestResultDialogComponent],
})
export class ObjectDetectionModule {}
