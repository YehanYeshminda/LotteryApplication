import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../../../../environments/environment.development";
import {MegaDrawResponse} from "../models/megaDraw";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MegaDrawHttpService {
  baseUrl:string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMegaDraws(countOfNumber: number  ): Observable<MegaDrawResponse> {
    return this.http.get<MegaDrawResponse>(this.baseUrl + 'Generator/MegaDrawValues', {
      params: {
        until: countOfNumber
      }
    });
  }
}
