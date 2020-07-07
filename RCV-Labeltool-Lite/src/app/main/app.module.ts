import { HttpClientModule } from '@angular/common/http'
import { NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { NgxsReduxDevtoolsPluginModule } from '@ngxs/devtools-plugin'
import { NgxsRouterPluginModule } from '@ngxs/router-plugin'
import { NgxsModule } from '@ngxs/store'

import { CommonModule } from '@common'
import { ConfigurationModule } from '@domain/configuration'
import { environment } from '@environment'
import { RcvImageModule, RcvObjectclassModule, RcvTopicModule } from '@rcv/domain'
import { BusyIndicatorModule, NotificationModule } from '@rcv/ui'
import { ImageModule } from '../features/image/image.module'
import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './components/app.component'

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserAnimationsModule,
    BrowserModule,
    BusyIndicatorModule,
    CommonModule,
    ConfigurationModule.forRoot({ configurationFile: environment.configurationFile }),
    HttpClientModule,
    NotificationModule,
    RcvTopicModule.forRoot(environment.services.mock),
    RcvObjectclassModule.forRoot(environment.services.mock),
    RcvImageModule.forRoot(environment.services.mock),
    ImageModule,

    // routes
    AppRoutingModule,

    // store
    NgxsModule.forRoot([], { developmentMode: !environment.production }),
    NgxsReduxDevtoolsPluginModule.forRoot(),
    NgxsRouterPluginModule.forRoot(),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
