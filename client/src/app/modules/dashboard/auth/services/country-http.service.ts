import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegionInfoResponse } from '../models/auth';
import { environment } from '@env/environment.development';

@Injectable({
  providedIn: 'root'
})
export class CountryHttpService {
  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAllCountryCodes(): Observable<RegionInfoResponse[]> {
    return this.http.get<RegionInfoResponse[]>(this.baseUrl + "Region");
  }
}
