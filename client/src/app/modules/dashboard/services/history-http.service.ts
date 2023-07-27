import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SearchBasedOnHistory, UserHistoryResponse } from '../user-history/models/userhistory';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class HistoryHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  searchForHistories(data: SearchBasedOnHistory): Observable<UserHistoryResponse[]> {
    return this.http.post<UserHistoryResponse[]>(this.baseUrl + 'History/Search-Based-OnHistory', data)
  }
}
