import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  HostBinding,
  OnDestroy,
  OnInit,
} from '@angular/core'
import { Observable, Subscription } from 'rxjs'

import { BusyIndicatorService } from '../busy-indicator.service'

@Component({
  selector: 'rcv-busy-indicator',
  templateUrl: './busy-indicator.component.html',
  styleUrls: ['./busy-indicator.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BusyIndicatorComponent implements OnInit, OnDestroy {
  private _visibilitySubscription: Subscription

  @HostBinding('class.hidden') hidden = false

  constructor(private cd: ChangeDetectorRef, private srv: BusyIndicatorService) {}

  ngOnInit() {
    this._visibilitySubscription = this.srv.IsVisible$.subscribe(isVisible => {
      this.hidden = !isVisible
      // this is required, else the loading indicator might not hide
      this.cd.detectChanges()
    })
  }

  ngOnDestroy() {
    this._visibilitySubscription.unsubscribe()
  }

  get Message$(): Observable<string | undefined> {
    return this.srv.Message$
  }

  get Progress$(): Observable<number | undefined> {
    return this.srv.Progress$
  }
}
