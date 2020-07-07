import { MessageBoxActionButton } from './message-box-action-button.interface'

export interface MessageBoxParamsInterface {
  header: string
  message: string
  buttons: MessageBoxActionButton[]
}
