import { Component, OnDestroy, OnInit } from '@angular/core';
import { login } from './modules/dashboard/auth/features/auth.actions';
import { Store, select } from '@ngrx/store';
import { AppState } from './reducer';
import { isLoggedIn } from './modules/dashboard/auth/features/auth.selectors';
import { interval, Observable, of, Subscription } from 'rxjs';
import { GetNotificationResponse, SendNotificationHttpService } from "./shared/alerts/send-notification-http.service";
import { confirmApproveNotification } from "./shared/alerts/sweetalert";
import { BsModalRef, BsModalService, ModalOptions } from "ngx-bootstrap/modal";
import { NotificationDialogComponent } from "./components/notification-dialog/notification-dialog.component";
import { CookieService } from 'ngx-cookie-service';
import { getAuthDetails } from '@shared/methods/methods';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(private store: Store<AppState>, private notificationHttpService: SendNotificationHttpService, private modalService: BsModalService, private cookieService: CookieService) { }

  isLoggedIn$: Observable<boolean> = of(false);
  private intervalDurationInMilliseconds = 1 * 10 * 60000;
  private intervalSubscription!: Subscription;
  private intervalStartTime!: number;
  bsModalRef?: BsModalRef;

  ngOnInit(): void {
    const user = getAuthDetails(this.cookieService.get('user'));
    if (user != null) {
      if (user.role === "ADMIN") {
        const storedStartTime = localStorage.getItem('intervalStartTime');
        if (storedStartTime) {
          this.intervalStartTime = parseInt(storedStartTime, 10);
        } else {
          this.intervalStartTime = Date.now();
          localStorage.setItem('intervalStartTime', this.intervalStartTime.toString());
        }

        const elapsed = Date.now() - this.intervalStartTime;
        const initialDelay = Math.max(0, this.intervalDurationInMilliseconds - (elapsed % this.intervalDurationInMilliseconds));

        this.intervalSubscription = interval(this.intervalDurationInMilliseconds)
          .subscribe(() => {
            this.sendNotification();
          });

        setTimeout(() => {
          this.sendNotification();
        }, initialDelay);
      }
    }

    const userProfile = localStorage.getItem("user");

    if (userProfile) {
      this.store.dispatch(login({ user: JSON.parse(userProfile) }));
    }

    this.isLoggedIn$ = this.store
      .pipe(
        select(isLoggedIn)
      );
  }

  openModalWithComponent(data: GetNotificationResponse[]) {
    const initialState: ModalOptions = {
      initialState: {
        formData: data
      }
    };
    this.bsModalRef = this.modalService.show(NotificationDialogComponent, initialState);
  }

  sendNotification() {
    this.notificationHttpService.sendNotification().subscribe({
      next: data => {
        confirmApproveNotification("New Report!", "Yes! Generate Report!", "No! Don't Generate Report!").then(response => {
          if (response.isConfirmed) {
            this.openModalWithComponent(data);
          }
        })
      }
    }
    );
  }

  ngOnDestroy(): void {
    if (this.intervalSubscription) {
      this.intervalSubscription.unsubscribe();
    }
  }
}
