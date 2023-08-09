import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../../../../environments/environment.development";
import { BuyEasyDraw, EasyDrawResponse, FullEasyDraw, GetDrawResult, Root } from "../models/EasyDrawResponse";
import { AuthDetails } from "../../../../../shared/models/auth";
import { getAuthDetails } from '@shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class EasyDrawHttpService {
  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getEasyDrawNumbers(auth: AuthDetails): Observable<EasyDrawResponse> {
    return this.http.post<EasyDrawResponse>(this.baseUrl + 'Generator/EasyDraw', {
      hash: auth.hash,
    })
  }

  getEasyDraw(): Observable<FullEasyDraw> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<FullEasyDraw>(this.baseUrl + 'Draw/GetEasyDrawInfo', auth)
  }

  buyEasyDrawNo(buyEasyDraw: BuyEasyDraw): Observable<Root<GetDrawResult>> {
    return this.http.post<Root<GetDrawResult>>(this.baseUrl + 'Draw/BuyDraws', buyEasyDraw);
  }
}
