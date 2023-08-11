import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment.development';
import { AuthDetails } from '@shared/models/auth';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';
import { StatusCheckData } from '../models/statuscheck';
import { getAuthDetails } from '@shared/methods/methods';

@Injectable({
  providedIn: 'root'
})
export class StatusCheckHttpService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getAllStatus(): Observable<StatusCheckData[]> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<StatusCheckData[]>(this.baseUrl + "Order", auth);
  }
}
