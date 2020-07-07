import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { Observable, throwError as _throw } from 'rxjs'
import { catchError, finalize, map } from 'rxjs/operators'

import { BusyIndicatorService } from './busy-indicator.service'

@Injectable()
export class BusyIndicatorInterceptor implements HttpInterceptor {
  constructor(private busyIndicatorService: BusyIndicatorService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.busyIndicatorService.start()

    return next.handle(req).pipe(
      map(e => e),
      catchError(e => _throw(e)),
      finalize(() => this.busyIndicatorService.stop()),
    )
  }
}
