import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { ConfigurationServiceConfig } from './configuration-service-config'
import { Configuration } from './configuration.interface'

@Injectable({ providedIn: 'root' })
export class ConfigurationService {
  cfg: Configuration

  constructor(private config: ConfigurationServiceConfig, private http: HttpClient) {}

  create(): Promise<Configuration> {
    let fileName = this.config.configurationFile
    if (fileName[0] !== '/') {
      fileName = '/' + fileName
    }
    return this.http
      .get<Configuration>('./assets' + fileName)
      .toPromise()
      .then(cfg => (this.cfg = cfg))
  }
}

export function createConfiguration(srv: ConfigurationService) {
  return () => srv.create()
}
