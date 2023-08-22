import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { getAuthDetails } from '@shared/methods/methods';
import { AuthDetails } from '@shared/models/auth';
import { CookieService } from 'ngx-cookie-service';
import { Root } from 'src/app/modules/dashboard/components/easy-draw/models/EasyDrawResponse';
import { MakeWithdrawalRequest, WithDrawal } from '../models/withdraw';
import { Observable } from 'rxjs';
import { environment } from '@env/environment.development';

@Injectable({
  providedIn: 'root'
})
export class WithdrawHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  makeWithdrawalRequest(data: MakeWithdrawalRequest): Observable<Root<WithDrawal>> {
    return this.http.post<Root<WithDrawal>>(this.baseUrl + "Upi/MakeUpiWithDrawalRequest", data);
  }
}
