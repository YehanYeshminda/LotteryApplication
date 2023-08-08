import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {GetLotto} from "../models/lotto";
import {CookieService} from "ngx-cookie-service";
import {getAuthDetails} from "@shared/methods/methods";
import {AuthDetails} from "@shared/models/auth";
import {environment} from "@env/environment.development";

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
}
