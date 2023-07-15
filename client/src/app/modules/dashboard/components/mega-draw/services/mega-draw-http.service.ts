import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { MegaDrawResponse } from '../models/megaDraw';

@Injectable({
  providedIn: 'root'
})
export class MegaDrawHttpService {

  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getDraws(until: number) {
    return this.http.get<MegaDrawResponse>(this.baseUrl + "Generator/MegaDrawValues", {
      params: {
        until: until
      }
    });
  }
}
