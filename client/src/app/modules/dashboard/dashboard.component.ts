import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { RouterReducerState } from '@ngrx/router-store';
import {Observable} from "rxjs";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  routerState$: Observable<RouterReducerState<any>> | undefined;

  constructor(private store: Store<{ router: RouterReducerState<any> }>) {}

  ngOnInit(): void {
    this.routerState$ = this.store.pipe(select('router'));
  }
}
