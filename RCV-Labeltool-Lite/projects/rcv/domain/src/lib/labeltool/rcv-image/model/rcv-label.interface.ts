export interface RcvLabelInterface {
  Id: string
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

  isActive?: boolean
  isSelected?: boolean
  isVisible?: boolean
}
