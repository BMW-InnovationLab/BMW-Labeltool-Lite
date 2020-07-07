import { Injectable } from '@angular/core'
import { Observable, Subject } from 'rxjs'
import { filter } from 'rxjs/operators'

import { ErrorCollector } from './error-collector'
import { ErrorInterface, ErrorSubject } from './error.interface'

@Injectable({
  providedIn: 'root',
})
export class ErrorCollectionService {
  private errorSubject: ErrorSubject = new Subject<ErrorInterface>()

  get All$() {
    return this.errorSubject.asObservable()
  }

  collectFor(scopeName: string): ErrorCollector {
    return new ErrorCollector(this.errorSubject, scopeName)
  }

  ofScope$(scope: string): Observable<ErrorInterface> {
    return this.All$.pipe(filter(value => value.scope === scope))
  }

  ofType$(type: any): Observable<ErrorInterface> {
    return this.All$.pipe(filter(value => value.type === type))
  }
}
