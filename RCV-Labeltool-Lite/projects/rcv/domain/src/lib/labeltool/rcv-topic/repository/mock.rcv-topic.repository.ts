import { Injectable } from '@angular/core'
import { BehaviorSubject, Observable } from 'rxjs'

import { RcvTopicInterface } from '../rcv-topic.interface'
import { AbstractRcvTopicRepository } from './abstract.rcv-topic.repository'

@Injectable()
export class MockRcvTopicRepository extends AbstractRcvTopicRepository {
  private topics: RcvTopicInterface[] = [
    {
      Id: -1,
      Name: 'Baustelle',
      Path: 'Robotron',
    },
    {
      Id: 0,
      Name: 'Sub_Baustelle',
      Path: 'Robotron/Baustelle',
    },
    {
      Id: 1,
      Name: 'Behaelter',
      Path: 'BMW',
    },
    {
      Id: 2,
      Name: 'bsh_l82_kis_io',
      Path: 'BSH',
    },
    {
      Id: 3,
      Name: 'bsh_l82_kis_nio',
      Path: 'BSH',
    },
    {
      Id: 4,
      Name: 'bsh_l83_kil_io',
      Path: 'BSH',
    },
    {
      Id: 5,
      Name: 'bsh_l83_kil_nio',
      Path: 'BSH',
    },
    {
      Id: 6,
      Name: 'bsh_l83_kis_io',
      Path: 'BSH',
    },
    {
      Id: 7,
      Name: 'bsh_l83_kis_nio',
      Path: 'BSH',
    },
    {
      Id: 8,
      Name: 'HoloLens',
      Path: 'BMW/HoloLens',
    },
    {
      Id: 9,
      Name: 'HTW_Bilder_Leiterplatte_mit_Loch',
      Path: 'HTW/Leiterplatten',
    },
    {
      Id: 10,
      Name: 'HTW_Bilder_Leiterplatte_ohne_Loch',
      Path: 'HTW/Leiterplatten',
    },
    {
      Id: 11,
      Name: 'Path is empty',
      Path: '',
    },
    {
      Id: 12,
      Name: 'Path is null',
      Path: undefined,
    },
    {
      Id: 13,
      Name: 'Path does not exist',
    },
  ]

  private topicSubject = new BehaviorSubject<RcvTopicInterface[]>(this.topics)

  get Topics$(): Observable<RcvTopicInterface[]> {
    return this.topicSubject.asObservable()
  }
}
