import { CommonModule as AngularCommonModule } from '@angular/common'
import { NgModule } from '@angular/core'
import { NgxsModule } from '@ngxs/store'

import { CommonModule } from '@common'
import { ImageDomainModule } from '@domain/image'
import { TopicDomainModule, TopicState } from '@domain/topic'
import { HelpDialogComponent } from './components/help-dialog/help-dialog.component'
import { ImageNavigationComponent } from './container/image-navigation/image-navigation.component'
import { ImagePageComponent } from './container/image-page/image-page.component'
import { ImageSidenavComponent } from './container/image-sidenav/image-sidenav.component'
import { TopicSelectDialogComponent } from './container/topic-select-dialog/topic-select-dialog.component'
import { ImageRoutingModule } from './image-routing.module'

@NgModule({
  imports: [
    AngularCommonModule,
    CommonModule,
    TopicDomainModule,

    // store
    ImageDomainModule,
    NgxsModule.forFeature([TopicState]),

    // routing
    ImageRoutingModule,
  ],
  declarations: [
    HelpDialogComponent,
    ImageNavigationComponent,
    ImagePageComponent,
    ImageSidenavComponent,
    TopicSelectDialogComponent,
  ],
  entryComponents: [HelpDialogComponent, TopicSelectDialogComponent],
})
export class ImageModule {}
