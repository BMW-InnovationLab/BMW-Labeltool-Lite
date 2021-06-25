import { RcvLabelInterface } from '@rcv/domain/labeltool-client'

export interface ILabel extends RcvLabelInterface {
  isActive: boolean
  isSelected: boolean
  isVisible: boolean
  isNew?: boolean
}
