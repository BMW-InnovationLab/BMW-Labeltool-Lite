import { Injectable } from '@angular/core'
import { Observable } from 'rxjs/index'

import { RcvTopicInterface } from '../../rcv-topic'
import { RcvImageResponseInterface } from '../model/rcv-image-response.interface'
import { RcvLabelMode } from '../model/rcv-label-mode.enum'

@Injectable()
export abstract class AbstractRcvImageRepository {
  abstract image(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
  ): Observable<RcvImageResponseInterface>

  abstract blank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface>

  abstract lastBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface>

  abstract next(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface>

  abstract nextBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface>

  abstract previous(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface>

  abstract previousBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface>
}
