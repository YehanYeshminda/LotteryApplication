import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MakeLogin, MakeRegisterUser, User } from '../models/user';
import { Observable } from 'rxjs';
import { OtpSend } from '../models/auth';
import { GetVerifyOtpRequestResponse, VerifyOtpRequest } from '../models/otp';
import { environment } from '@env/environment.development';

interface GetOtpResponse {
  number: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  login(user: MakeLogin): Observable<User> {
    return this.http.post<User>(this.baseUrl + "Account/Login", user);
  }

  registerUser(user: MakeRegisterUser): Observable<User> {
    return this.http.post<User>(this.baseUrl + "Account/Register", user);
  }

  sendOtp(sendOtp: OtpSend): Observable<GetOtpResponse> {
    return this.http.post<GetOtpResponse>(this.baseUrl + "Account/SendOtp", sendOtp);
  }

  verifyOtp(otp: VerifyOtpRequest): Observable<GetVerifyOtpRequestResponse> {
    return this.http.post<GetVerifyOtpRequestResponse>(this.baseUrl + "Account/VerifyOTP", otp);
  }
}
