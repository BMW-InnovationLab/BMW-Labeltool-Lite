import { Observable } from 'rxjs'

import { RcvTopicInterface } from '../rcv-topic.interface'

export abstract class AbstractRcvTopicRepository {
  abstract get Topics$(): Observable<RcvTopicInterface[]>
}
