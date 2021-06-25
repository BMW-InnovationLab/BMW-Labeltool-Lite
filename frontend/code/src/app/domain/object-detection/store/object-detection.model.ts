import { ILabel } from '../models'

export interface ObjectDetectionStateModel {
  allLabelsVisible: boolean
  hovered?: ILabel
  labels: ILabel[]
  strokeWidth: number
  fill: boolean
}
