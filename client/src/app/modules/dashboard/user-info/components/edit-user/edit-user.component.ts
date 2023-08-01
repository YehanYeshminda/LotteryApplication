import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { SingleUserInfo, UpdateSingleUserInfo } from '../../models/single-user';
import { AppState } from 'src/app/reducer';
import { Store } from '@ngrx/store';
import { selectSingleUserInfo } from '../../features/user-info.selectors';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SingleUserHttpService } from '../../services/single-user-http.service';
import { AuthDetails } from 'src/app/shared/models/auth';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { confirmApproveNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit {
  singleUserInfo$: Observable<SingleUserInfo | undefined> = of();
  form: FormGroup = new FormGroup({});

  constructor(private store: Store<AppState>, private fb: FormBuilder, private singleUserHttpService: SingleUserHttpService, private cookieService: CookieService, private router: Router) { }

  ngOnInit(): void {
    this.intializeForm();
    this.singleUserInfo$ = this.store.select(selectSingleUserInfo);

    this.singleUserInfo$.subscribe({
      next: response => {
        if (!response) return;
        this.form.patchValue(response)
      }
    })
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

    console.log(values);

    this.singleUserHttpService.updateSingleUserHistory(values).subscribe({
      next: response => {
        successNotification(`User with the email ${response.custName} been successfully Edited`)
      },
      error: err => {
        console.log(err)
      }
    })
  }

  navigateToCancel() {
    if (this.form.dirty) {
      confirmApproveNotification("You will lose all you changes if you navigate back!").then(response => {
        if (response.isConfirmed) {
          this.router.navigateByUrl("/dashboard/home")
        }
      })
    } else {
      this.router.navigateByUrl("/dashboard/home")
    }
  }
}
