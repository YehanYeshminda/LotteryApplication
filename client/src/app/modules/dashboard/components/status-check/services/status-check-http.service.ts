import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment.development';
import { AuthDetails } from '@shared/models/auth';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';
import { MakeRequestToCheckUpdate, StatusCheckData } from '../models/statuscheck';
import { getAuthDetails } from '@shared/methods/methods';

export interface GetReturnStatus {
  status: string
}

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

  updateStatus(orderReferenceId: string): Observable<GetReturnStatus> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    const data: MakeRequestToCheckUpdate = {
      authDto: auth,
      transactionId: orderReferenceId
    }

    return this.http.post<GetReturnStatus>(this.baseUrl + "upi/checkstatusupi", data);
  }
}
