import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/reducer';
import { AuthHttpService } from './services/auth-http.service';
import { noop, Observable, of, tap } from 'rxjs';
import { login } from './features/auth.actions';
import { MakeLogin, User } from './models/user';
import { OtpSend } from './models/auth';
import { errorNotification } from 'src/app/shared/alerts/sweetalert';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { VerifyOtpComponent } from './components/verify-otp/verify-otp.component';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  registerMode: boolean = false;
  otpSent: boolean = false;
  disabledTime: Observable<number> = of(0);
  isDisabled: boolean = false;
  remainingTime: number = 30;
  bsModalRef?: BsModalRef;
  @ViewChild('timerSpan', { static: false }) timerSpan!: ElementRef;

  constructor(private fb: FormBuilder, private router: Router, private store: Store<AppState>, private authService: AuthHttpService, private modalService: BsModalService
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    if (!this.registerMode) {
      this.form = this.fb.group({
        custName: ['', [Validators.required]],
        password: ['', [Validators.required]],
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
        password: ['', [Validators.required]],
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      if (!this.registerMode) {
        const data: MakeLogin = {
          username: this.form.value.custName,
          password: this.form.value.password
        }

        this.isDisabled = true;
        this.authService
          .login(data)
          .pipe(
            tap((user) => {
              this.store.dispatch(login({ user }));
              this.router.navigate(['/dashboard/home']).then(() => {
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
      } else if (this.registerMode) {
        const data: User = { ...this.form.value }

        this.isDisabled = true;
        this.authService
          .registerUser(data)
          .pipe(
            tap((user) => {
              this.store.dispatch(login({ user }));
              this.router.navigate(['/dashboard/home']).then(() => {
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
      }
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
