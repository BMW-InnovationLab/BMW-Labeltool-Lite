import { ScrollingModule } from '@angular/cdk/scrolling'
import { NgModule } from '@angular/core'

import { CommonModule } from '@common'
import { ImageDomainModule } from '@domain/image'
import { TopicDomainModule, TopicState } from '@domain/topic'
import { NgxsModule } from '@ngxs/store'
import { FileUploadModule, I18NModule, ThemeLoaderModule } from '@rcv/ui'
import { CircleChartComponent } from './components/circle-chart/circle-chart.component'
import { HelpDialogComponent } from './components/help-dialog/help-dialog.component'
import { ImageDataModalComponent } from './components/image-data-modal/image-data-modal.component'
import { ImageThumbnailComponent } from './components/image-thumbnail/image-thumbnail.component'
import { ImageNavigationComponent } from './container/image-navigation/image-navigation.component'
import { ImagePageComponent } from './container/image-page/image-page.component'
import { ImageSidenavComponent } from './container/image-sidenav/image-sidenav.component'
import { TopicSelectDialogComponent } from './container/topic-select-dialog/topic-select-dialog.component'
import { ImageRoutingModule } from './image-routing.module'

@NgModule({
  imports: [
    CommonModule,
    FileUploadModule,
    I18NModule,
    TopicDomainModule,

    // store
    ImageDomainModule,
    NgxsModule.forFeature([TopicState]),

    // routing
    ImageRoutingModule,
    ScrollingModule,
    ThemeLoaderModule,
  ],
  declarations: [
    HelpDialogComponent,
    ImageDataModalComponent,
    ImageNavigationComponent,
    ImagePageComponent,
    ImageSidenavComponent,
    TopicSelectDialogComponent,
    CircleChartComponent,
    ImageThumbnailComponent,
  ],
  entryComponents: [HelpDialogComponent, ImageDataModalComponent, TopicSelectDialogComponent],
})
export class ImageModule {}
