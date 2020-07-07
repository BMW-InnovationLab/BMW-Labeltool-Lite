import { Subject } from 'rxjs'

export interface ErrorInterface {
  scope: string
  type: any
  message: string
  data?: any
}

export type ErrorSubject = Subject<ErrorInterface>
