import { CommonModule as AngularCommonModule } from '@angular/common'
import { NgModule } from '@angular/core'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import {
  MatButtonModule,
  MatButtonToggleModule,
  MatCardModule,
  MatCheckboxModule,
  MatDialogModule,
  MatDividerModule,
  MatExpansionModule,
  MatFormFieldModule,
  MatIconModule,
  MatInputModule,
  MatListModule,
  MatMenuModule,
  MatOptionModule,
  MatProgressBarModule,
  MatProgressSpinnerModule,
  MatSelectModule,
  MatSidenavModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSnackBarModule,
  MatTableModule,
  MatTabsModule,
  MatToolbarModule,
  MatTooltipModule,
  MatTreeModule,
} from '@angular/material'
import { RouterModule } from '@angular/router'

import { ConfirmButtonComponent } from './components/confirm-button/confirm-button.component'
import { ConfirmModalComponent } from './components/confirm-modal/confirm-modal.component'
import { SaveActionsComponent } from './components/save-actions/save-actions.component'

const exportedModules = [
  // angular
  FormsModule,
  ReactiveFormsModule,

  // material
  MatButtonModule,
  MatButtonToggleModule,
  MatCardModule,
  MatCheckboxModule,
  MatDialogModule,
  MatDividerModule,
  MatExpansionModule,
  MatFormFieldModule,
  MatIconModule,
  MatInputModule,
  MatListModule,
  MatMenuModule,
  MatOptionModule,
  MatProgressBarModule,
  MatProgressSpinnerModule,
  MatSelectModule,
  MatSidenavModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSnackBarModule,
  MatTableModule,
  MatTabsModule,
  MatToolbarModule,
  MatTooltipModule,
  MatTreeModule,
]

const exportedComponents = [ConfirmButtonComponent, ConfirmModalComponent, SaveActionsComponent]

@NgModule({
  imports: [AngularCommonModule, RouterModule, ...exportedModules],
  declarations: [...exportedComponents],
  entryComponents: [ConfirmModalComponent],
  exports: [...exportedModules, ...exportedComponents],
})
export class CommonModule {}
