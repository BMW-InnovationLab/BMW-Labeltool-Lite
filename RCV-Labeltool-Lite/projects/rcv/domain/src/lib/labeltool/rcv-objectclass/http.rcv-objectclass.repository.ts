import { Injectable } from '@angular/core'
import { ApiConnectionFactory, HttpApiCollectionConnection } from '@rcv/common'
import { from, Observable } from 'rxjs'

import { LabeltoolConfigurationService } from '../configuration/configuration.service'
import { RcvTopicInterface } from '../rcv-topic'
import { AbstractRcvObjectclassRepository } from './abstract.rcv-objectclass.repository'
import { RcvObjectclassInterface } from './rcv-objectclass.interface'

@Injectable()
export class HttpRcvObjectclassRepository extends AbstractRcvObjectclassRepository {
  private connection: HttpApiCollectionConnection<RcvObjectclassInterface>

  constructor(private connector: ApiConnectionFactory, config: LabeltoolConfigurationService) {
    super()
    config.CurrentApiUrl$.subscribe(apiUrl => {
      this.connection = this.connector.ConnectToCollection(apiUrl, 'objectclasses')
    })
  }

  Objectclasses$(topic: RcvTopicInterface): Observable<RcvObjectclassInterface[]> {
    return from(this.connection.Api.httpGet<RcvObjectclassInterface[]>(String(topic.Id)))
  }
}
