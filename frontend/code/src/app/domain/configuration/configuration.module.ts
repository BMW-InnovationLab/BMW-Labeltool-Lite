import { APP_INITIALIZER, ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core'

import { Configuration } from '@rcv/domain/labeltool-client'
import { ThemeLoaderService } from '@rcv/ui'
import { ConfigurationServiceConfig } from './configuration-service-config'
import { ConfigurationService, createConfigurationAndThemes } from './configuration.service'

const configurationFactory = () => {
  return new Configuration()
}

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
          useFactory: createConfigurationAndThemes,
          deps: [ConfigurationService, ThemeLoaderService],
          multi: true,
        },
        { provide: ConfigurationServiceConfig, useValue: config },
        { provide: Configuration, useFactory: configurationFactory },
      ],
      ngModule: ConfigurationModule,
    }
  }
}
