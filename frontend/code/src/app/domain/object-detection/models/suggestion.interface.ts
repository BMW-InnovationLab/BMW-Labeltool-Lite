import { RcvModelInterface, RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { IService } from './service.interface'

export interface ISuggestion {
  service: IService
  model: RcvModelInterface
  objectClass?: RcvObjectClassViewInterface
}
