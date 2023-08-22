import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { errorNotification, successNotification } from '@shared/alerts/sweetalert';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { WithdrawHttpService } from './services/withdraw-http.service';
import { AuthDetails } from '@shared/models/auth';
import { getAuthDetails } from '@shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { MakeWithdrawalRequest } from './models/withdraw';

@Component({
  selector: 'app-withdraw-dialog',
  templateUrl: './withdraw-dialog.component.html',
  styleUrls: ['./withdraw-dialog.component.scss']
})
export class WithdrawDialogComponent implements OnInit {
  latitude: number = 0;
  longitude: number = 0;
  form: FormGroup = new FormGroup({});

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder, private withDrawHttpService: WithdrawHttpService, private cookieService: CookieService) { }

  ngOnInit(): void {
    this.intializeForm();
  }

  makeWithdrawalRequest() {
    if ('geolocation' in navigator && this.form.valid) {
      navigator.geolocation.getCurrentPosition(
        position => {
          this.form.controls['longitude'].setValue(position.coords.longitude.toString());
          this.form.controls['latitude'].setValue(position.coords.latitude.toString());
          this.makeWithdrawRequest();
        },
        error => {
          errorNotification(error.message);
        }
      );
    } else {
      errorNotification("Geolocation is not available in this browser.");
    }
  }

  makeWithdrawRequest() {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));

    if (auth != null) {
      const data: MakeWithdrawalRequest = {
        ...this.form.value,
        authDto: auth,
      }

      this.withDrawHttpService.makeWithdrawalRequest(data).subscribe({
        next: response => {
          if (response.isSuccess) {
            successNotification(response.message)
          } else {
            errorNotification(response.message);
          }
        }
      })
    } else {
      errorNotification("Invalid Authentication Details");
    }

  }

  intializeForm() {
    this.form = this.fb.group({
      benificiaryAccountNo: ['', [Validators.required]],
      benificiaryIfscCode: ['', [Validators.required]],
      benificiaryName: ['', [Validators.required]],
      amount: [null, [Validators.required]],
      longitude: ['', []],
      latitude: ['', []],
    })
  }
}
