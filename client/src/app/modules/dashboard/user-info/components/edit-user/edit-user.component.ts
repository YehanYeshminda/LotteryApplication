import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, of, Subject, takeUntil } from 'rxjs';
import { SingleUserInfo, UpdateSingleUserInfo } from '../../models/single-user';
import { AppState } from 'src/app/reducer';
import { Store } from '@ngrx/store';
import { selectSingleUserInfo } from '../../features/user-info.selectors';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SingleUserHttpService } from '../../services/single-user-http.service';
import { AuthDetails } from 'src/app/shared/models/auth';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit, OnDestroy {
  singleUserInfo$: Observable<SingleUserInfo | undefined> = of();
  form: FormGroup = new FormGroup({});
  private destroy$ = new Subject<void>();

  constructor(private store: Store<AppState>, private fb: FormBuilder, private singleUserHttpService: SingleUserHttpService, private cookieService: CookieService) { }

  ngOnInit(): void {
    this.intializeForm();
    this.singleUserInfo$ = this.store.select(selectSingleUserInfo);

    this.singleUserInfo$.pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: response => {
        if (!response) return;
        this.form.patchValue(response);
      }
    });
  }

  intializeForm() {
    this.form = this.fb.group({
      custName: ['', [Validators.required]],
      email: ['', [Validators.required]],
      contactNo: ['', [Validators.required]],
      alternatePhone: ['', [Validators.required]],
      custAddress: ['', [Validators.required]],
      nic: ['', [Validators.required]]
    })
  }


  getUserTimeZone() {
    try {
      if ('Intl' in window && 'DateTimeFormat' in Intl) {
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        return timeZone;
      } else {
        const offset = new Date().getTimezoneOffset();
        const positiveOffset = Math.abs(offset);
        const hours = String(Math.floor(positiveOffset / 60)).padStart(2, '0');
        const minutes = String(positiveOffset % 60).padStart(2, '0');
        const sign = offset > 0 ? '-' : '+';
        return `${sign}${hours}:${minutes}`;
      }
    } catch (error) {
      console.error('Error while getting the user time zone:', error);
      return 'UTC';
    }
  }

  updateUserInfo() {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    const values: UpdateSingleUserInfo = {
      ...this.form.value,
      authDto: auth
    };

    this.singleUserHttpService.updateSingleUserHistory(values).subscribe({
      next: response => {
        if (response == null) {
          errorNotification("User with this information already exist!");
        } else {
          successNotification(`User with the email ${response.email} been successfully Edited`)
        }
      },
    })
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
