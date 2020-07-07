import { IEntity } from '@core'
import { IModel } from './model.interface'

export interface IService extends IEntity {
  Name: string
  Type: string

  imageClassificationModels?: IModel[]
  objectDetectionModels?: IModel[]

  DetectObjectClasses: boolean
  SupportsObjectDetection: boolean
  SupportsImageClassification: boolean
  SupportsImageSegmentation: boolean
}
