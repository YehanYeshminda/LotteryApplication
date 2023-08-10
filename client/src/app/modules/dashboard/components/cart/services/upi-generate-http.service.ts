import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment.development';
import { HttpHeaders } from '@ngrx/data/src/dataservices/interfaces';
import { getAuthDetails } from '@shared/methods/methods';
import { AuthDetails } from '@shared/models/auth';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';

export interface GetUpiPerson {
  username: string
  password: string
}

export interface BaseResponse {
  responseCode: number
  success: boolean
  message: string
  data: Data
}

export interface Data {
  token: string
  partner: Partner
}

export interface Partner {
  id: number
  name: string
  email: string
  phone: string
}


// CONTINUE FROM HERE
export interface BaseResponseFromUPi {
  responseCode: number
  success: boolean
  message: string
  data: DataBackFromUPi
}

export interface DataBackFromUPi {
  qr: string
  pinWalletTransactionId: string
  userTrasnactionId: string
  status: string
  statusMessage: string
  statusCode: string
}

export interface DataToSendUpi {
  Name: string
  ReferenceId: string
  Email: string
  Phone: string
  amount: string
}

@Injectable({
  providedIn: 'root'
})
export class UpiGenerateHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  getUPIUser(): Observable<GetUpiPerson> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<GetUpiPerson>(this.baseUrl + "Payment/AuthAndGenToken", auth)
  }

  getAuthDetails(data: GetUpiPerson): Observable<BaseResponse> {
    const sendData: GetUpiPerson = {
      username: data.username,
      password: data.password
    };

    return this.http.post<BaseResponse>("https://app.pinwallet.in/api/token/create", sendData, {
      headers: {
        "IPAddress": "16.16.243.167"
      }
    })
  }

  generateUpi(data: DataToSendUpi, token: string, password: string): Observable<BaseResponseFromUPi> {
    return this.http.post<BaseResponseFromUPi>("https://app.pinwallet.in/api/DyupiV2/V4/GenerateUPI", data, {
      headers: {
        "Authorization": `Bearer ${token}`,
        "IPAddress": "16.16.243.167",
        "AuthKey": password
      }
    })
  }
}
