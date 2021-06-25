import { ILabel } from '@domain/object-detection'
import { Box } from '../components/box'
import { IPoint } from '../models/point.interface'

export class ElementBuilder {
  static setBoxGeometry(
    box: Box,
    startPoint: IPoint,
    endPoint: IPoint,
    startBackPoint?: IPoint,
    endBackPoint?: IPoint,
  ) {
    box.Start = startPoint
    box.End = endPoint
    if (startBackPoint != null && endBackPoint != null) {
      box.StartBack = startBackPoint
      box.EndBack = endBackPoint
      box.createBackElement(false)
    }
  }

  static pointsFromLabel(label: ILabel) {
    const startPoint: IPoint = { x: label.Left, y: label.Top }
    const endPoint: IPoint = { x: label.Right, y: label.Bottom }
    return [startPoint, endPoint]
  }

  static backPointsFromLabel(label: ILabel) {
    const startPoint: IPoint = label.LeftBack == null ? undefined : { x: label.LeftBack, y: label.TopBack }
    const endPoint: IPoint = label.RightBack == null ? undefined : { x: label.RightBack, y: label.BottomBack }
    return [startPoint, endPoint]
  }
}
