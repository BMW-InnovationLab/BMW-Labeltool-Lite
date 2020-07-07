import { Injectable } from '@angular/core'
import { ReplaySubject } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class LabeltoolConfigurationService {
  private _currentApiUrl$ = new ReplaySubject<string>(1)

  get CurrentApiUrl$() {
    return this._currentApiUrl$.asObservable()
  }

  constructor() {}

  updateApiUrl(value: string) {
    this._currentApiUrl$.next(value)
  }
}
