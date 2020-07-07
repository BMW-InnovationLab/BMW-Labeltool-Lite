import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { map } from 'rxjs/operators'

import { fillPath } from '@rcv/common'
import { LabeltoolConfigurationService } from '../../configuration/configuration.service'
import { RcvTopicInterface } from '../../rcv-topic'
import { RcvImageResponseInterface } from '../model/rcv-image-response.interface'
import { RcvLabelMode } from '../model/rcv-label-mode.enum'
import { AbstractRcvImageRepository } from './abstract.rcv-image.repository'

@Injectable()
export class HttpRcvImageRepository extends AbstractRcvImageRepository {
  private apiUrl: string

  constructor(private http: HttpClient, config: LabeltoolConfigurationService) {
    super()
    config.CurrentApiUrl$.subscribe(apiUrl => (this.apiUrl = apiUrl))
  }

  get imageApiUrl(): string {
    return `${this.apiUrl}/images`
  }

  get navigateApiUrl(): string {
    return `${this.apiUrl}/navigate`
  }

  image(mode: RcvLabelMode, topic: RcvTopicInterface, index: number): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, false, index)
  }

  blank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, shuffle, index, 'blank')
  }

  lastBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, shuffle, undefined, 'lastblank')
  }

  next(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, shuffle, index, 'next')
  }

  nextBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, shuffle, index, 'nextblank')
  }

  previous(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, shuffle, index, 'previous')
  }

  previousBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return this.get(mode, topic, shuffle, index, 'previousblank')
  }

  private get(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    shuffle: boolean,
    index?: number,
    operation?: string,
  ): Observable<RcvImageResponseInterface> {
    const url = `${this.navigateApiUrl}/${topic.Id}`

    return this.http
      .get<RcvImageResponseInterface>(operation ? `${url}/${operation}` : url, {
        params: {
          index: String(index),
          labelMode: mode,
          shuffle: String(shuffle),
        },
      })
      .pipe(map(image => ({ ...image, ImageLabel: fillPath(image.ImageLabel, 'Path', 'Url', this.apiUrl) })))
  }
}
