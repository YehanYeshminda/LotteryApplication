import { Component, OnInit } from '@angular/core';
import { login } from './modules/dashboard/auth/features/auth.actions';
import { Store, select } from '@ngrx/store';
import { AppState } from './reducer';
import { isLoggedIn } from './modules/dashboard/auth/features/auth.selectors';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  constructor(private store: Store<AppState>) { }
  isLoggedIn$: Observable<boolean> = of(false);

  ngOnInit(): void {
    const userProfile = localStorage.getItem("user");

    if (userProfile) {
      this.store.dispatch(login({ user: JSON.parse(userProfile) }));
    }

    this.isLoggedIn$ = this.store
      .pipe(
        select(isLoggedIn)
      );
  }
}
