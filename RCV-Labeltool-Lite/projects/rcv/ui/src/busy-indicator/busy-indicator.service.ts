import { Injectable } from '@angular/core'
import { BehaviorSubject, Observable } from 'rxjs'

@Injectable()
export class BusyIndicatorService {
  private _isVisibleSubject = new BehaviorSubject<boolean>(false)
  private _messageSubject = new BehaviorSubject<string | undefined>(undefined)
  private _progressSubject = new BehaviorSubject<number | undefined>(undefined)

  private set requests(val: number) {
    this._requests = val
    this._isVisibleSubject.next(this._requests > 0)
  }
  // tslint:disable-next-line
  private get requests() {
    return this._requests
  }

  private _requests = 0

  setMessage(message?: string) {
    this._messageSubject.next(message)
  }

  setProgress(progress?: number) {
    this._progressSubject.next(progress)
  }

  start(message?: string) {
    if (message != null) {
      this.setMessage(message)
    }

    ++this.requests
  }

  stop() {
    --this.requests
  }

  get Message$(): Observable<string | undefined> {
    return this._messageSubject.asObservable()
  }

  get Message(): string | undefined {
    return this._messageSubject.getValue()
  }

  get Progress$(): Observable<number | undefined> {
    return this._progressSubject.asObservable()
  }

  get Progress(): number | undefined {
    return this._progressSubject.getValue()
  }

  get IsVisible$(): Observable<boolean> {
    return this._isVisibleSubject.asObservable()
  }

  get IsVisible(): boolean {
    return this._isVisibleSubject.getValue()
  }

  cancel() {
    this.requests = 0
  }
}
