import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VerifyOtpRequest } from '../../models/otp';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AuthHttpService } from '../../services/auth-http.service';
import { errorNotification } from 'src/app/shared/alerts/sweetalert';

@Component({
  selector: 'app-verify-otp',
  templateUrl: './verify-otp.component.html',
  styleUrls: ['./verify-otp.component.scss']
})
export class VerifyOtpComponent implements OnInit {
  form: FormGroup = new FormGroup({});

  constructor(private fb: FormBuilder, private bsModalRef: BsModalRef, private authHttpService: AuthHttpService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      otp: ['', [Validators.required]]
    })
  }

  verifyOtp() {
    const values: VerifyOtpRequest = {
      ...this.form.value
    }

    this.authHttpService.verifyOtp(values).subscribe({
      next: response => {
        if (response.status === true) {
          this.bsModalRef.hide();
        } else {
          errorNotification("Wrong OTP! Please enter a valid OTP!")
        }
      }
    })
  }
}
