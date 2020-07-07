import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { ErrorCollectionService } from '../error-collector/error-collection.service'
import { HttpApiCollectionConnection } from './http.api.collection.connection'
import { HttpApiConnection } from './http.api.connection'

@Injectable({ providedIn: 'root' })
export class ApiConnectionFactory {
  constructor(private logger: ErrorCollectionService, private http: HttpClient) {}

  ConnectTo(baseUrl: string, ...entitySegments: string[]): HttpApiConnection {
    return new HttpApiConnection(this.http, baseUrl, entitySegments)
  }

  ConnectToCollection<TModel>(
    baseUrl: string,
    ...entitySegments: string[]
  ): HttpApiCollectionConnection<TModel> {
    const collector = this.logger.collectFor('Some Collection')
    return new HttpApiCollectionConnection<TModel>(collector, this.ConnectTo(baseUrl, ...entitySegments))
  }

  ConfigureCollection<TModel>(
    baseUrl: string,
    collection?: HttpApiCollectionConnection<TModel>,
    ...entitySegments: string[]
  ): HttpApiCollectionConnection<TModel> {
    if (collection == null) {
      return this.ConnectToCollection(baseUrl, ...entitySegments)
    }

    collection.Api.baseUrl = baseUrl
    collection.reload()
    return collection
  }
}
