import { RcvLabelInterface } from './rcv-label.interface'
import { RcvSegmentInterface } from './rcv-segment.interface'

export interface RcvImageInterface {
  Height: number
  Id: string
  Index: number
  Labels: RcvLabelInterface[]
  Path: string
  Segments: RcvSegmentInterface[]
  Url: string
  Width: number
}
