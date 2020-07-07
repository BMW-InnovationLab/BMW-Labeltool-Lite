import { Injectable } from '@angular/core'
import { Observable, of } from 'rxjs'
import { delay } from 'rxjs/operators'

import { RcvTopicInterface } from '@rcv/domain'
import { ILabel, IModel, IService } from '../models'
import { AbstractObjectDetectionService } from './object-detection-service'

const models = [
  {
    Id: '1',
    Name: 'Test Model 1',
  },
  {
    Id: '2',
    Name: 'Test Model 2',
  },
]

@Injectable()
export class ObjectDetectionServiceMock extends AbstractObjectDetectionService {
  imageClassificationModels(service: IService): Observable<IModel[]> {
    return of(models)
  }

  imageSegmentationModels(service: IService): Observable<IModel[]> {
    return of(models)
  }

  objectDetectionModels(service: IService): Observable<IModel[]> {
    return of(models)
  }

  saveOrUpdate(topic: RcvTopicInterface, imageId: string, labels: ILabel[]): Observable<any> {
    return of(true).pipe(delay(2000))
  }

  protected fetchServices(): Observable<IService[]> {
    return of([
      {
        Id: 'digitsbmwrcvsouthcentralus',
        Name: 'Development DIGITS Server',
        Type: 'ObjectDetector',
        DetectObjectClasses: false,
        SupportsObjectDetection: true,
        SupportsImageClassification: true,
        SupportsImageSegmentation: true,
        imageSegmentationModels: models,
        objectDetectionModels: models,
      },
      {
        Id: 'hololens',
        Name: 'HOLOLENS DIGITS Server',
        Type: 'NVIDIA DIGITS',
        DetectObjectClasses: false,
        SupportsObjectDetection: false,
        SupportsImageClassification: true,
        SupportsImageSegmentation: false,
        imageSegmentationModels: models,
        objectDetectionModels: models,
      },
      {
        Id: 'digitsbmwrcvwesteurope',
        Name: 'DIGITS 6',
        Type: 'NVIDIA DIGITS',
        DetectObjectClasses: true,
        SupportsObjectDetection: true,
        SupportsImageClassification: false,
        SupportsImageSegmentation: false,
        imageSegmentationModels: models,
        objectDetectionModels: models,
      },
    ])
  }
}
