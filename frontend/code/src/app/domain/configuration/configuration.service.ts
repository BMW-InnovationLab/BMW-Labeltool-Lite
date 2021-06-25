import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { LabeltoolConfigurationService } from '@rcv/domain'

import { Configuration as LabeltoolConfiguration } from '@rcv/domain/labeltool-client'
import { ThemeLoaderService } from '@rcv/ui'
import { version } from '../../../environments/version'
import { ConfigurationServiceConfig } from './configuration-service-config'
import { Configuration } from './configuration.interface'

@Injectable({ providedIn: 'root' })
export class ConfigurationService {
  cfg: Configuration

  constructor(
    private config: ConfigurationServiceConfig,
    private http: HttpClient,
    private labeltoolConfig: LabeltoolConfigurationService,
    private labeltoolConfiguration: LabeltoolConfiguration,
  ) {}

  create(): Promise<Configuration> {
    let fileName = this.config.configurationFile
    if (fileName[0] !== '/') {
      fileName = '/' + fileName
    }
    return this.http
      .get<Configuration>('./assets' + fileName)
      .toPromise()
      .then(cfg => {
        this.labeltoolConfig.updateApiUrl(cfg.apiUrl)
        this.labeltoolConfiguration.basePath = cfg.apiUrl.split('/api')[0]
        return (this.cfg = cfg)
      })
  }

  load<T>(section?: string): T {
    let config = this.cfg
    if (section != null) {
      config = config[section]
    }
    return <T>(<any>config)
  }
}

export function createConfigurationAndThemes(srv: ConfigurationService, themeLoader: ThemeLoaderService) {
  return async () => {
    await srv.create()
    themeLoader.initialize(['bmw'], 'rcv-cockpit.theme', `v=${version}`)
    await themeLoader.loadDynamicTheme()
  }
}
