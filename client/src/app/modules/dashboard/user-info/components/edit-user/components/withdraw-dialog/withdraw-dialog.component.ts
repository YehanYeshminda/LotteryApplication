import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { errorNotification, successNotification } from '@shared/alerts/sweetalert';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { WithdrawHttpService } from './services/withdraw-http.service';
import { AuthDetails } from '@shared/models/auth';
import { getAuthDetails } from '@shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { ComboData, EditExistingBankDetails, MakeRequestToForBankDetials, MakeWithdrawalRequest } from './models/withdraw';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-withdraw-dialog',
  templateUrl: './withdraw-dialog.component.html',
  styleUrls: ['./withdraw-dialog.component.scss']
})
export class WithdrawDialogComponent implements OnInit {
  latitude: number = 0;
  longitude: number = 0;
  form: FormGroup = new FormGroup({});
  bankDetails$: Observable<ComboData[]> = of([]);
  isEdit: boolean = false;

  constructor(public bsModalRef: BsModalRef, private fb: FormBuilder, private withDrawHttpService: WithdrawHttpService, private cookieService: CookieService) { }

  ngOnInit(): void {
    this.intializeForm();
    this.bankDetails$ = this.withDrawHttpService.getBankDetails();
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
      errorNotification("Please enter valid details");
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

  editExistingBank() {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));

    if (auth !== null) {
      const data: EditExistingBankDetails = {
        ...this.form.value,
        authDto: auth
      }

      if (this.form.valid) {
        this.withDrawHttpService.editExistingBankDetails(data).subscribe({
          next: response => {
            if (response.isSuccess) {
              successNotification(response.message);
            } else {
              errorNotification(response.message)
            }
          }
        })

      } else {
        errorNotification("Please enter the required fields")
      }
    } else {
      errorNotification("Invaid Login");
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
      upiId: ['', []],
      bankId: ['', []]
    })

    this.form.controls['bankId'].valueChanges.subscribe({
      next: response => {
        if (response !== 0 || response !== '') {
          const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));

          if (auth !== null) {
            var data: MakeRequestToForBankDetials = {
              authDto: auth,
              id: response
            }

            this.withDrawHttpService.getExistingBankDetailByID(data).subscribe({
              next: response => {
                this.form.controls['benificiaryAccountNo'].setValue(response.benificiaryAccountNo);
                this.form.controls['benificiaryIfscCode'].setValue(response.benificiaryIfscCode);
                this.form.controls['benificiaryName'].setValue(response.benificiaryName);
                this.form.controls['upiId'].setValue(response.upiid);
                this.form.controls['amount'].setValue(0);

                this.isEdit = true;
              },
              error: err => {
                console.log(err);
              }
            })
          }
        }
      }
    })
  }
}
