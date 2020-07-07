import { RcvImageInterface } from './rcv-image.interface'

export interface RcvImageResponseInterface {
  HasNext: boolean
  HasNextBlank: boolean
  HasPrevious: boolean
  HasPreviousBlank: boolean
  ImageCount: number
  ImageLabel: RcvImageInterface
}
