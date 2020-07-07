import { ButtonLabel } from './button-label.enum'

export interface MessageBoxActionButton {
  label: string | ButtonLabel
  result: any
  warning?: boolean
}
