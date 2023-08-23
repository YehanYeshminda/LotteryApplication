import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment.development';
import { getAuthDetails } from '@shared/methods/methods';
import { AuthDetails } from '@shared/models/auth';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';
import { LottoHistory } from '../models/lottoHistory';

@Injectable({
  providedIn: 'root'
})
export class LottoHistoryHttpService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getAllLottoHistory(): Observable<LottoHistory[]> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<LottoHistory[]>(this.baseUrl + "Lotto/GetLottoTransactionHistory", auth);
  }
}
