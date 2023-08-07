import {Component, OnDestroy, OnInit} from '@angular/core';
import { login } from './modules/dashboard/auth/features/auth.actions';
import { Store, select } from '@ngrx/store';
import { AppState } from './reducer';
import { isLoggedIn } from './modules/dashboard/auth/features/auth.selectors';
import {interval, Observable, of, Subscription} from 'rxjs';
import {SendNotificationHttpService} from "./shared/alerts/send-notification-http.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(private store: Store<AppState>, private notificationHttpService: SendNotificationHttpService) { }
  isLoggedIn$: Observable<boolean> = of(false);
  private notificationSubscription!: Subscription;

  ngOnInit(): void {
    const intervalDurationInMilliseconds = 1 * 10 * 10000; // 10 minutes in milliseconds

    this.notificationSubscription = interval(intervalDurationInMilliseconds).subscribe(() => {
      this.sendNotification();
    });

    const userProfile = localStorage.getItem("user");

    if (userProfile) {
      this.store.dispatch(login({ user: JSON.parse(userProfile) }));
    }

    this.isLoggedIn$ = this.store
      .pipe(
        select(isLoggedIn)
      );
  }

  sendNotification() {
    this.notificationHttpService.sendNotification().subscribe(
      (response) => {

      },
      (error) => {
        console.error('Error sending notification:', error);
      }
    );
  }

  ngOnDestroy() {
    this.notificationSubscription.unsubscribe();
  }
}
