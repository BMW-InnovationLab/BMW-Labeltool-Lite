import { ErrorSubject } from './error.interface'

export class ErrorCollector {
  constructor(private subj: ErrorSubject, private scope: string) {}

  emit(type: string, message: string, data?: any) {
    this.subj.next({
      type: 'jj',
      scope: this.scope,
      message: message,
      data: data,
    })
  }
}
