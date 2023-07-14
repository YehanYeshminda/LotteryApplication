import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MakeLogin, User } from '../models/user';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { OtpSend } from '../models/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  login(user: MakeLogin): Observable<User> {
    return this.http.post<User>(this.baseUrl + "Account/Login", user);
  }

  registerUser(user: User): Observable<User> {
    return this.http.post<User>(this.baseUrl + "Account/Register", user);
  }

  sendOtp(sendOtp: OtpSend): Observable<number> {
    return this.http.post<number>(this.baseUrl + "Account/SendOtp", sendOtp);
  }
}
