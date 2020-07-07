import { enableProdMode } from '@angular/core'
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic'
import 'hammerjs'

import { AppInjector } from '@core/libraries/base-component/app-injector'
import { AppModule } from './app/main/app.module'
import { environment } from './environments/environment'

if (environment.production) {
  enableProdMode()
}

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .then(moduleRef => AppInjector.setInjector(moduleRef.injector))
  // tslint:disable-next-line
  .catch(err => console.log(err))
