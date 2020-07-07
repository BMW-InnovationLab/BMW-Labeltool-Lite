import { CommonModule } from '@angular/common'
import { NgModule } from '@angular/core'
import { MatSelectModule } from '@angular/material'

import { BoundingBoxEditorComponent } from './bounding-box-editor.component'
import { ElementService } from './lib/element.service'

@NgModule({
  imports: [CommonModule, MatSelectModule],
  declarations: [BoundingBoxEditorComponent],
  providers: [ElementService],
  exports: [BoundingBoxEditorComponent],
})
export class BoundingBoxEditorModule {}
