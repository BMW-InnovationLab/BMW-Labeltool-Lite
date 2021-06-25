import { RcvModelInterface, RcvServiceInterface } from '@rcv/domain/labeltool-client'

export interface IService extends RcvServiceInterface {
  objectDetectionModels?: RcvModelInterface[]
}
