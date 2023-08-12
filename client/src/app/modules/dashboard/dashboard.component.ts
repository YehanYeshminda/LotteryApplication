import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { RouterReducerState } from '@ngrx/router-store';
import { Observable, of } from "rxjs";
import { Router } from '@angular/router';
import { StatusCheckHttpService } from './components/status-check/services/status-check-http.service';
import { StatusCheckData } from './components/status-check/models/statuscheck';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  routerState$: Observable<RouterReducerState<any>> | undefined;
  loading!: boolean;
  loadingValue: number = 0;
  searchTerm: string = '';

  constructor(private store: Store<{ router: RouterReducerState<any> }>, private router: Router) { }

  ngOnInit(): void {
    this.routerState$ = this.store.pipe(select('router'));
    // this.routerState$.subscribe({
    //   next: response => {
    //     if (response.navigationId === 2) {
    //       this.loading = false;
    //       // this.startLoading();
    //     }
    //   }
    // })
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

    const duration = 4300;
    const interval = 10;
    const steps = duration / interval;

    let step = 0;
    const increment = 150 / steps;

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
