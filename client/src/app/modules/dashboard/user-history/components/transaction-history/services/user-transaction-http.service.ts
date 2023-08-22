import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Root } from 'src/app/modules/dashboard/components/easy-draw/models/EasyDrawResponse';
import { UserLosingTransaction, UserTransaction } from '../models/transaction';
import { AuthDetails } from '@shared/models/auth';
import { getAuthDetails } from '@shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '@env/environment.development';

@Injectable({
  providedIn: 'root'
})
export class UserTransactionHttpService {
  baseUrl = environment.apiUrl;

  constructor(private Http: HttpClient, private cookieService: CookieService) { }

  getAllUserWinningTransactions(): Observable<Root<UserTransaction[]>> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.Http.post<Root<UserTransaction[]>>(this.baseUrl + "History/GetUserTransactionHistory", auth);
  }

  getAllUserLosingTransactions(): Observable<Root<UserLosingTransaction[]>> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.Http.post<Root<UserLosingTransaction[]>>(this.baseUrl + "History/GetUserTransactionLoserHistory", auth);
  }
}
