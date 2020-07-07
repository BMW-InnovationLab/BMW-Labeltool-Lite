import { Observable } from 'rxjs'

import { RcvTopicInterface } from '../rcv-topic/rcv-topic.interface'
import { RcvObjectclassInterface } from './rcv-objectclass.interface'

export abstract class AbstractRcvObjectclassRepository {
  abstract Objectclasses$(topic: RcvTopicInterface): Observable<RcvObjectclassInterface[]>
}
