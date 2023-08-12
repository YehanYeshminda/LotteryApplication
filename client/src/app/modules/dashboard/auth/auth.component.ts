import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/reducer';
import { AuthHttpService } from './services/auth-http.service';
import { noop, tap } from 'rxjs';
import { login } from './features/auth.actions';
import { MakeLogin, MakeRegisterUser, User } from './models/user';
import { OtpSend } from './models/auth';
import { errorNotification } from 'src/app/shared/alerts/sweetalert';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { VerifyOtpComponent } from './components/verify-otp/verify-otp.component';
import { getRandomCaptcha } from "@shared/methods/methods";

interface PopulateFields {
  Value: number
  text: string
}

interface PopulateFieldsWithStrings {
  code: string;
  name: string;
}

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  registerMode: boolean = false;
  otpSent: boolean = false;
  isDisabled: boolean = false;
  remainingTime: number = 30;
  bsModalRef?: BsModalRef;
  captcha: string = '';
  numberCode = '+91'
  @ViewChild('timerSpan', { static: false }) timerSpan!: ElementRef;

  constructor(private fb: FormBuilder, private router: Router, private store: Store<AppState>, private authService: AuthHttpService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    if (!this.registerMode) {
      this.form = this.fb.group({
        custName: ['', [Validators.required]],
        custPassword: ['', [Validators.required]],
        captcha: ['', [Validators.required]],
        enteredCaptcha: ['', [Validators.required]],
      });
    } else {
      this.form = this.fb.group({
        custName: ['', [Validators.required]],
        nic: ['', []],
        email: ['', [Validators.required, Validators.email]],
        custAddress: ['', [Validators.required]],
        mobile: ['', [Validators.required]],
        alternatePhone: ['', [Validators.required]],
        contactNo: ['', [Validators.required]],
        dob: [new Date(), []],
        custPassword: ['', [Validators.required]],
      });
    }

    if (!this.registerMode) {
      this.captcha = getRandomCaptcha();
      this.form.controls['captcha'].setValue(this.captcha);
    }
  }

  generateCaptcha(): void {
    this.captcha = getRandomCaptcha();
    this.form.controls['captcha'].setValue(this.captcha);
  }

  onSubmit() {
    if (!this.registerMode) {
      const validCaptcha = this.form.controls['enteredCaptcha'].value == this.captcha;
      if (this.form.valid && validCaptcha) {
        const data: MakeLogin = {
          username: this.form.value.custName,
          password: this.form.value.custPassword
        }

        this.isDisabled = true;
        this.authService
          .login(data)
          .pipe(
            tap((user) => {
              this.store.dispatch(login({ user }));
              this.router.navigate(['/dashboard/user-home']).then(() => {
                this.isDisabled = false;
              });
            })
          )
          .subscribe({
            next: noop,
            error: (error) => {
              errorNotification(error.error);
              this.isDisabled = false;
            }
          });
      } else {
        errorNotification("Captcha does not match");
      }
    } else if (this.registerMode) {
      const data: MakeRegisterUser = {
        ...this.form.value,
        contactNo: this.numberCode + this.form.value.contactNo.toString(),
        alternatePhone: this.numberCode + this.form.value.alternatePhone.toString(),
      }

      console.log(data)

      this.isDisabled = true;
      this.authService
        .registerUser(data)
        .pipe(
          tap((user) => {
            this.isDisabled = false;
            this.registerMode = false;
          })
        )
        .subscribe({
          next: (respones) => {
            console.log(respones)
            this.registerMode = false;

            if (this.registerMode == false) {
              this.initializeForm();
              this.generateCaptcha();
            }
          },
          error: (error) => {
            errorNotification(error.error);
            this.isDisabled = false;
          }
        });
    }
  }

  openModalWithComponent() {
    this.bsModalRef = this.modalService.show(VerifyOtpComponent);
  }

  sendOtp() {
    if (this.form.controls['mobile'].invalid) {
      console.error("Missing Mobile Number!")
      return;
    }

    const values: OtpSend = {
      phoneNumber: this.form.controls['mobile'].value,
      method: "sms"
    }

    this.authService.sendOtp(values).subscribe({
      next: response => {
        if (response) {
          this.otpSent = !this.otpSent;
          this.startTimer();
        }
      },
      complete: () => {
        this.openModalWithComponent();
        setTimeout(() => {
          this.otpSent = !this.otpSent;
        }, 50000);
      }
    })

    // this.otpSent = true;
  }

  toggleRegisterMode() {
    this.registerMode = !this.registerMode;
    this.initializeForm();
  }

  startTimer() {
    let interval = setInterval(() => {
      this.remainingTime--;

      const minutes = Math.floor(this.remainingTime / 60);
      const seconds = this.remainingTime % 60;
      console.log(this.remainingTime)

      this.timerSpan.nativeElement.textContent = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

      if (this.remainingTime <= 0) {
        clearInterval(interval);
        this.otpSent = false;
      }
    }, 1000);
  }
}
