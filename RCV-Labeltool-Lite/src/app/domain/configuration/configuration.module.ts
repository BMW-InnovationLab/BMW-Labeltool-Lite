import { APP_INITIALIZER, ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core'
import { ConfigurationService, createConfiguration } from './configuration.service'

import { ConfigurationServiceConfig } from './configuration-service-config'

@NgModule({
  imports: [],
  providers: [ConfigurationService],
})
export class ConfigurationModule {
  constructor(
    @Optional()
    @SkipSelf()
    parentModule: ConfigurationModule,
  ) {
    if (parentModule) {
      throw new Error('ConfigurationModule is already loaded. Import only once')
    }
  }

  static forRoot(config: ConfigurationServiceConfig): ModuleWithProviders {
    return {
      providers: [
        {
          provide: APP_INITIALIZER,
          useFactory: createConfiguration,
          deps: [ConfigurationService],
          multi: true,
        },
        { provide: ConfigurationServiceConfig, useValue: config },
      ],
      ngModule: ConfigurationModule,
    }
  }
}
