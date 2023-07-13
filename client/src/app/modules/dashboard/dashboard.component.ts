import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { RouterReducerState } from '@ngrx/router-store';
import { Observable } from "rxjs";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  routerState$: Observable<RouterReducerState<any>> | undefined;
  loading!: boolean;
  loadingValue: number = 0;

  constructor(private store: Store<{ router: RouterReducerState<any> }>) { }

  ngOnInit(): void {
    this.routerState$ = this.store.pipe(select('router'));
    this.loading = true;
    this.startLoading();
  }

  setDelayedLoadingValue(value: number, delay: number): void {
    setTimeout(() => {
      if (value < 100) {
        this.loadingValue = value;
      } else if (value === 100) {
        this.loading = false;
      }
    }, delay);
  }

  startLoading(): void {
    this.loading = true;

    const duration = 4000;
    const interval = 10;
    const steps = duration / interval;

    let step = 0;
    const increment = 100 / steps;

    const intervalId = setInterval(() => {
      step++;
      this.loadingValue = step * increment;

      if (step === steps) {
        clearInterval(intervalId);
        this.loading = false;
      }
    }, interval);
  }

}
