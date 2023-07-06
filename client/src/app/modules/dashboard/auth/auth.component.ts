import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/reducer';
import { AuthHttpService } from './services/auth-http.service';
import { noop, tap } from 'rxjs';
import { login } from './features/auth.actions';
import { MakeLogin } from './models/user';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { OtpSend } from './models/auth';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {
  form: FormGroup = new FormGroup({});
  registerMode: boolean = false;
  modalRef?: BsModalRef;

  constructor(private fb: FormBuilder, private router: Router, private store: Store<AppState>, private authService: AuthHttpService,
    private modalService: BsModalService
    ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    if (this.registerMode == false) {
      this.form = this.fb.group({
        custName: ['', [Validators.required]],
        password: ['', []],
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
        city: ['', [Validators.required]],
        state: ['', [Validators.required]],
        zip: ['', [Validators.required]],
        password: ['', [Validators.required]],
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      if (this.registerMode == false) {
        const data: MakeLogin = {
          username: this.form.value.custName,
          password: this.form.value.password
        }

        this.authService
          .login(data)
          .pipe(
            tap((user) => {
              this.store.dispatch(login({ user }));
              this.router.navigate(['/dashboard']);
            })
          )
          .subscribe({
            next: noop,
            error: (err) => alert('Login Failed')
          });
      } else {
        this.router.navigate(['/dashboard']);
      }
    }
  }

  sendOtp() {
    if (this.form.controls['mobile'].invalid) {
      console.error("Missing Mobile Number!")
    }

    const values: OtpSend = {
      phoneNumber: this.form.controls['mobile'].value,
      method: "sms"
    }

    this.authService.sendOtp(values).subscribe({
      next: response => {
        console.log(response);
      }
    })
  }

  toggleRegisterMode() {
    this.registerMode = !this.registerMode;
    this.initializeForm();
  }
}
