import { RcvModelInterface, RcvTopicViewInterface } from '@rcv/domain/labeltool-client'
import { RcvLabelViewInterface } from '@rcv/domain/labeltool-client/model/label-view.interface'
import { Observable } from 'rxjs'
import { first } from 'rxjs/operators'
import { ILabel, IService } from '../models'

export abstract class AbstractObjectDetectionService {
  abstract objectDetectionModels(service: IService): Observable<RcvModelInterface[]>

  services(): Observable<IService[]> {
    return this.fetchServices()
  }

  abstract saveOrUpdate(topic: RcvTopicViewInterface, imageId: string, labels: ILabel[]): Observable<ILabel[]>

  abstract suggest(
    topic: RcvTopicViewInterface,
    imageId: string,
  ): Observable<Array<RcvLabelViewInterface> | null>

  async loadObjectDetectionModels(service: IService): Promise<IService> {
    if (!service.objectDetectionModels) {
      if (service.SupportsObjectDetection) {
        service.objectDetectionModels = await this.objectDetectionModels(service)
          .pipe(first())
          .toPromise()
      } else {
        service.objectDetectionModels = []
      }
    }
    return service
  }

  protected abstract fetchServices(): Observable<IService[]>
}
