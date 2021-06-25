import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { ConfigurationService } from '@domain/configuration'
import { RcvModelInterface, RcvTopicViewInterface, SuggestService } from '@rcv/domain/labeltool-client'
import { RcvLabelViewInterface } from '@rcv/domain/labeltool-client/model/label-view.interface'
import { Observable, of } from 'rxjs'
import { catchError } from 'rxjs/operators'
import { ILabel, IService } from '../models'
import { AbstractObjectDetectionService } from './object-detection-service'

@Injectable()
export class ObjectDetectionService extends AbstractObjectDetectionService {
  private readonly API_URL = this.configurationService.cfg.apiUrl

  constructor(
    private configurationService: ConfigurationService,
    private http: HttpClient,
    private suggestService: SuggestService,
  ) {
    super()
  }

  objectDetectionModels(service: IService): Observable<RcvModelInterface[]> {
    return this.http
      .get<RcvModelInterface[]>(`${this.API_URL}/model/objectdetection/${service.Id}`)
      .pipe(catchError(e => of([])))
  }

  saveOrUpdate(topic: RcvTopicViewInterface, imageId: string, labels: ILabel[]): Observable<ILabel[]> {
    const url = `${this.API_URL}/images/${topic.Id}/${imageId}/labels`
    // clear ids of new labels
    const labelsWithClearedIds = labels.map(l => ({ ...l, Id: l.isNew ? undefined : l.Id }))
    return this.http.put<ILabel[]>(url, labelsWithClearedIds)
  }

  suggest(topic: RcvTopicViewInterface, imageId: string): Observable<Array<RcvLabelViewInterface> | null> {
    return this.suggestService.getBoundingBoxes(topic.Id, imageId)
  }

  protected fetchServices(): Observable<IService[]> {
    return this.http.get<IService[]>(this.API_URL + '/service')
  }
}
