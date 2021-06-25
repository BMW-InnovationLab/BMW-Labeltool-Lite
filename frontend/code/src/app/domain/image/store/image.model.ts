import { RcvImageResponseInterface, RcvLabelMode } from '@rcv/domain'

export interface ImageStateModel {
  imageReady: boolean
  imageResponse?: RcvImageResponseInterface
  imageSize: [number, number]
  defaultImageSize: [number, number]
  mode: RcvLabelMode
  zoom: number
  brightness: number
}
