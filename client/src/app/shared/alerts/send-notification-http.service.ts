import { Injectable } from '@angular/core';
import {environment} from "../../../environments/environment.development";
import {HttpClient} from "@angular/common/http";

interface ISendNotification {
  dateFrom: string
  dateTo: string
}

@Injectable({
  providedIn: 'root'
})
export class SendNotificationHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

    getCurrentIndianTime(): Date {
        // Calculate Indian Standard Time offset (IST is UTC+5:30)
        const istOffsetMinutes = 5 * 60 + 30;

        // Get the current UTC time and add the IST offset
        const utcNow = new Date();
        utcNow.setMinutes(utcNow.getMinutes() + istOffsetMinutes);
        return utcNow;
    }

    sendNotification() {
        const indianTime = this.getCurrentIndianTime();

        const dateFrom = new Date(indianTime);
        dateFrom.setMinutes(dateFrom.getMinutes() - 10);

        const dateTo = new Date(indianTime);

        const requestData: ISendNotification = {
            dateFrom: dateFrom.toISOString(),
            dateTo: dateTo.toISOString(),
        };

        console.log(requestData)

        return this.http.post(this.baseUrl + "Lotto/CheckForNumberNoti", requestData);
    }


}
