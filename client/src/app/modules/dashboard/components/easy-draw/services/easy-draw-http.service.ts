import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../../../../environments/environment.development";
import { BuyEasyDraw, EasyDrawResponse, GetDrawResult, Root } from "../models/EasyDrawResponse";
import { AuthDetails } from "../../../../../shared/models/auth";

@Injectable({
  providedIn: 'root'
})
export class EasyDrawHttpService {
  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getEasyDrawNumbers(auth: AuthDetails): Observable<EasyDrawResponse> {
    return this.http.post<EasyDrawResponse>(this.baseUrl + 'Generator/EasyDraw', {
      hash: auth.hash,
    })
  }

  buyEasyDrawNo(buyEasyDraw: BuyEasyDraw): Observable<Root<GetDrawResult>> {
    return this.http.post<Root<GetDrawResult>>(this.baseUrl + 'Draw/BuyDraws', buyEasyDraw);
  }
}
