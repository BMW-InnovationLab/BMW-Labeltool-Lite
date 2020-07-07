import { IEntity } from '@core'

export interface ILabel extends IEntity {
  ObjectClassName: string
  ObjectClassId: string

  SourceModel?: string

  Left: number
  Bottom: number
  Right: number
  Top: number
  Confidence?: number
  Prediction?: number
  PredictionClass?: string

  isActive: boolean
  isSelected: boolean
  isVisible: boolean
}
