import { Component, OnInit } from '@angular/core';
import { RouterReducerState } from '@ngrx/router-store';
import { Store, select } from '@ngrx/store';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-user-history',
  templateUrl: './user-history.component.html',
  styleUrls: ['./user-history.component.scss']
})
export class UserHistoryComponent implements OnInit {
  routerState$: Observable<RouterReducerState<any>> | undefined;

  constructor(private store: Store<{ router: RouterReducerState<any> }>) { }

  ngOnInit(): void {
    this.routerState$ = this.store.pipe(select('router'));
  }

}
