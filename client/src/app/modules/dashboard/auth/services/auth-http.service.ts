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
		return this.http.post<User>("https://localhost:5001/api/Account/Login", user);
	}

  sendOtp(sendOtp: OtpSend) {
    return this.http.post("https://localhost:5001/api/Account/SendOtp", sendOtp);
  }
}
