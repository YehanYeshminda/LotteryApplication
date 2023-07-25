import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaymentDoneModel, PaymentResponseModel } from '../../checkout/model/checkout';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class PaymentHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  makePayment(paymentDoneModel: PaymentDoneModel) {
    return this.http.post<PaymentResponseModel>(this.baseUrl + "Payment/Add-Payment", paymentDoneModel)
  }
}
