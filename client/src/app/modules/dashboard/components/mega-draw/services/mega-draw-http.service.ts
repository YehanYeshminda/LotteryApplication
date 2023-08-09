import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { FullMegaDraw, MegaDrawResponse } from '../models/megaDraw';
import { Observable } from 'rxjs';
import { AuthDetails } from '@shared/models/auth';
import { getAuthDetails } from '@shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class MegaDrawHttpService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getDraws(until: number) {
    return this.http.get<MegaDrawResponse>(this.baseUrl + "Generator/MegaDrawValues", {
      params: {
        until: until
      }
    });
  }

  getMegaDraw(): Observable<FullMegaDraw> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<FullMegaDraw>(this.baseUrl + 'Draw/GetMegaDrawInfo', auth)
  }
}
