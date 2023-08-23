import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Root } from 'src/app/modules/dashboard/components/easy-draw/models/EasyDrawResponse';
import { ComboData, EditExistingBankDetails, ExistingBankDetails, MakeRequestToForBankDetials, MakeWithdrawalRequest, WithDrawal } from '../models/withdraw';
import { Observable } from 'rxjs';
import { environment } from '@env/environment.development';
import { AuthDetails } from '@shared/models/auth';
import { getAuthDetails } from '@shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class WithdrawHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  makeWithdrawalRequest(data: MakeWithdrawalRequest): Observable<Root<WithDrawal>> {
    return this.http.post<Root<WithDrawal>>(this.baseUrl + "Upi/MakeUpiWithDrawalRequest", data);
  }

  getBankDetails(): Observable<ComboData[]> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<ComboData[]>(this.baseUrl + "Upi/GetBankID", auth)
  }

  getExistingBankDetailByID(data: MakeRequestToForBankDetials): Observable<ExistingBankDetails> {
    return this.http.post<ExistingBankDetails>(this.baseUrl + "Upi/GetExistingBankDetails", data);
  }

  editExistingBankDetails(data: EditExistingBankDetails) {
    return this.http.post<Root<null>>(this.baseUrl + "Upi/EditExistingUserBankDetails", data);
  }
}
