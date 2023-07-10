import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MakeLogin, User } from '../models/user';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { OtpSend } from '../models/auth';
import {observableToBeFn} from "rxjs/internal/testing/TestScheduler";

@Injectable({
  providedIn: 'root'
})
export class AuthHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  login(user: MakeLogin): Observable<User> {
		return this.http.post<User>("https://localhost:5001/api/Account/Login", user);
	}

  sendOtp(sendOtp: OtpSend) : Observable<number> {
    return this.http.post<number>("https://localhost:5001/api/Account/SendOtp", sendOtp);
  }
}
