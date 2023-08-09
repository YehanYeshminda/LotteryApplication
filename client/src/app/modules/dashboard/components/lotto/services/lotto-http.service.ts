import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { BuyLotto, GetLotto } from "../models/lotto";
import { CookieService } from "ngx-cookie-service";
import { getAuthDetails } from "@shared/methods/methods";
import { AuthDetails } from "@shared/models/auth";
import { environment } from "@env/environment.development";
import { Root } from '../../easy-draw/models/EasyDrawResponse';

export interface CompanyCode {
  companyCode: string;
}

export interface BuyLottoResult {
  lottoNumbers: string
  referenceId: string
  lottoUnqueReferenceId: string
}

@Injectable({
  providedIn: 'root'
})
export class LottoHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getLottoNo(): Observable<GetLotto> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<GetLotto>(this.baseUrl + "Lotto/GetLottoNumbers", auth)
  }

  buyLotto(lottoNo: string): Observable<Root<BuyLottoResult>> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    const buyLotto: BuyLotto = {
      authDto: auth,
      lottoNumber: lottoNo
    }
    return this.http.post<Root<BuyLottoResult>>(this.baseUrl + "Lotto/BuyLotto", buyLotto)
  }

  getCompanyCode(): Observable<CompanyCode> {
    return this.http.get<CompanyCode>(this.baseUrl + "Company/CompanyCode")
  }
}
