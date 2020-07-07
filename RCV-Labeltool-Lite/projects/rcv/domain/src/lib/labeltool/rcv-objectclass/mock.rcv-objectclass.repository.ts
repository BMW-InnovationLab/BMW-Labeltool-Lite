import { Injectable } from '@angular/core'
import { Observable, of } from 'rxjs'

import { RcvTopicInterface } from '../rcv-topic'
import { AbstractRcvObjectclassRepository } from './abstract.rcv-objectclass.repository'
import { RcvObjectclassInterface } from './rcv-objectclass.interface'

@Injectable()
export class MockRcvObjectclassRepository extends AbstractRcvObjectclassRepository {
  private objectclasses: RcvObjectclassInterface[] = [
    {
      Id: '1',
      Name: 'Auto',
      Color: 1,
      ColorCode: '212fed',
    },
    {
      Id: '2',
      Name: 'Bagger',
      Color: 2,
      ColorCode: '306b13',
    },
    {
      Id: '3',
      Name: 'Frontlader',
      Color: 3,
      ColorCode: 'e9e369',
    },
  ]

  Objectclasses$(topic: RcvTopicInterface): Observable<RcvObjectclassInterface[]> {
    return of(this.objectclasses)
  }
}
