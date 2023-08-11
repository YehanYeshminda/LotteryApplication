import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment.development';
import { CookieService } from 'ngx-cookie-service';
import { GenerateUpi, UpiResponse } from '../models/upi';
import { Observable } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class UpiGenerateHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getUpiQrCode(data: GenerateUpi): Observable<UpiResponse> {
    return this.http.post<UpiResponse>(this.baseUrl + "Upi/generate-upi", data)
  }
}
