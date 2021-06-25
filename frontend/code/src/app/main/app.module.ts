import { HttpClientModule } from '@angular/common/http'
import { LOCALE_ID, NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'

import { CommonModule } from '@common'
import { ConfigurationModule } from '@domain/configuration'
import { environment } from '@environment'
import { TranslateCompiler, TranslateModule } from '@ngx-translate/core'
import { NgxsReduxDevtoolsPluginModule } from '@ngxs/devtools-plugin'
import { NgxsModule } from '@ngxs/store'
import { RcvImageModule, RcvTopicModule } from '@rcv/domain'
import { BusyIndicatorModule, NotificationModule, ThemeLoaderModule } from '@rcv/ui'
import { TranslateMessageFormatCompiler } from 'ngx-translate-messageformat-compiler'
import { ImageModule } from '../features/image/image.module'
import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './components/app.component'

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserAnimationsModule,
    BrowserModule,
    ThemeLoaderModule.forRoot(),
    BusyIndicatorModule.forRoot(['thumbnail/labels']),
    CommonModule,
    ConfigurationModule.forRoot({
      configurationFile: environment.configurationFile,
    }),
    HttpClientModule,
    NotificationModule,
    RcvTopicModule.forRoot(environment.services.mock),
    RcvImageModule.forRoot(environment.services.mock),
    TranslateModule.forRoot({
      compiler: {
        provide: TranslateCompiler,
        useClass: TranslateMessageFormatCompiler,
      },
    }),
    ImageModule,

    // routes
    AppRoutingModule,

    // store
    NgxsModule.forRoot([], { developmentMode: !environment.production }),
    NgxsReduxDevtoolsPluginModule.forRoot(),
  ],
  providers: [
    {
      provide: LOCALE_ID,
      useValue: 'en',
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
