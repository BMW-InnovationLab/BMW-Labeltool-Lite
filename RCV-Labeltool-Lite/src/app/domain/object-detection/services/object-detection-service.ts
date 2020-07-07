import { forkJoin, Observable, of } from 'rxjs'
import { flatMap, map } from 'rxjs/operators'

import { RcvTopicInterface } from '@rcv/domain'
import { ILabel, IModel, IService } from '../models'

export abstract class AbstractObjectDetectionService {
  abstract imageClassificationModels(service: IService): Observable<IModel[]>
  abstract imageSegmentationModels(service: IService): Observable<IModel[]>
  abstract objectDetectionModels(service: IService): Observable<IModel[]>

  services(): Observable<IService[]> {
    return this.fetchServices().pipe(
      flatMap(services => this.loadImageClassificationModels(services)),
      flatMap(services => this.loadObjectDetectionModels(services)),
    )
  }

  abstract saveOrUpdate(topic: RcvTopicInterface, imageId: string, labels: ILabel[]): Observable<any>

  protected abstract fetchServices(): Observable<IService[]>

  protected loadImageClassificationModels(services: IService[]) {
    return forkJoin(
      services.map(service => {
        if (service.SupportsImageClassification) {
          return this.imageClassificationModels(service).pipe(
            map(models => ({ ...service, imageClassificationModels: models })),
          )
        } else {
          return of({ ...service, imageClassificationModels: [] })
        }
      }),
    )
  }

  protected loadObjectDetectionModels(services: IService[]) {
    return forkJoin(
      services.map(service => {
        if (service.SupportsObjectDetection) {
          return this.objectDetectionModels(service).pipe(
            map(models => ({ ...service, objectDetectionModels: models })),
          )
        } else {
          return of({ ...service, objectDetectionModels: [] })
        }
      }),
    )
  }
}
