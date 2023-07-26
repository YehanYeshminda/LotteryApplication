import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthDetails } from 'src/app/shared/models/auth';
import { environment } from 'src/environments/environment.development';
import { PagedList, UserHistoryResponse } from '../models/userhistory';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { getAuthDetails } from 'src/app/shared/methods/methods';

@Injectable({
  providedIn: 'root'
})
export class UserHistoryHttpService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getUserDrawHistoryById(data: PagedList): Observable<UserHistoryResponse[]> {
    return this.http.post<UserHistoryResponse[]>(this.baseUrl + "History/User-History", data);
  }

  getUserDrawHistoryWinnings() {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<UserHistoryResponse[]>(this.baseUrl + "History/User-History-Winnings", auth)
  }

  getUserDrawHistoryForLast3Days() {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<UserHistoryResponse[]>(this.baseUrl + 'History/User-History-Winnings-BasedOnDate', auth)
  }
}
