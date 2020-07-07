import { Injectable } from '@angular/core'
import { ApiConnectionFactory, HttpApiCollectionConnection } from '@rcv/common'
import { Observable } from 'rxjs'

import { LabeltoolConfigurationService } from '../../configuration/configuration.service'
import { RcvTopicInterface } from '../rcv-topic.interface'
import { AbstractRcvTopicRepository } from './abstract.rcv-topic.repository'

@Injectable()
export class HttpRcvTopicRepository extends AbstractRcvTopicRepository {
  private coll: HttpApiCollectionConnection<RcvTopicInterface>

  constructor(private connector: ApiConnectionFactory, config: LabeltoolConfigurationService) {
    super()
    config.CurrentApiUrl$.subscribe(apiUrl => {
      this.coll = this.connector.ConfigureCollection(apiUrl, this.coll, 'topics')
    })
  }

  get Topics$(): Observable<RcvTopicInterface[]> {
    return this.coll.Entries$
  }
}
