import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { Observable, of } from 'rxjs'
import { catchError } from 'rxjs/operators'

import { ConfigurationService } from '@domain/configuration'
import { RcvTopicInterface } from '@rcv/domain'
import { ILabel, IModel, IService } from '../models'
import { AbstractObjectDetectionService } from './object-detection-service'

@Injectable()
export class ObjectDetectionService extends AbstractObjectDetectionService {
  private readonly API_URL = this.configurationService.cfg.apiUrl

  constructor(private configurationService: ConfigurationService, private http: HttpClient) {
    super()
  }

  imageClassificationModels(service: IService): Observable<IModel[]> {
    return this.http
      .get<IModel[]>(`${this.API_URL}/model/imageclassification/${service.Id}`)
      .pipe(catchError(e => of([])))
  }

  imageSegmentationModels(service: IService): Observable<IModel[]> {
    return this.http
      .get<IModel[]>(`${this.API_URL}/model/imagesegmentation/${service.Id}`)
      .pipe(catchError(e => of([])))
  }

  objectDetectionModels(service: IService): Observable<IModel[]> {
    return this.http
      .get<IModel[]>(`${this.API_URL}/model/objectdetection/${service.Id}`)
      .pipe(catchError(e => of([])))
  }

  saveOrUpdate(topic: RcvTopicInterface, imageId: string, labels: ILabel[]): Observable<any> {
    const url = `${this.API_URL}/images/${topic.Id}/${imageId}/labels`
    // the server doesn't like our generated ids, so we delete them here
    const filteredLabels = labels.map(label => ({ ...label, Id: undefined }))

    return this.http.put(url, filteredLabels)
  }

  protected fetchServices(): Observable<IService[]> {
    return this.http.get<IService[]>(this.API_URL + '/service')
  }
}
