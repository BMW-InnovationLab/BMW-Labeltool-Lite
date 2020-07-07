import { RcvImageResponseInterface, RcvLabelMode } from '@rcv/domain'

export interface ImageStateModel {
  imageReady: boolean
  imageResponse?: RcvImageResponseInterface
  imageSize: [number, number]
  mode: RcvLabelMode
}
