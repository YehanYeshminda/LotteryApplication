import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {environment} from "../../../../../../environments/environment.development";
import {AuthDetails} from "../../../../../shared/models/auth";
import {EasyDrawResponse} from "../models/EasyDrawResponse";

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
}
