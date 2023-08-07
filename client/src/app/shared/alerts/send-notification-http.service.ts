import { Injectable } from '@angular/core';
import {environment} from "../../../environments/environment.development";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";

interface ISendNotification {
  dateFrom: string
  dateTo: string
}

export interface GetNotificationResponse {
    no: number
    count: number
}

@Injectable({
  providedIn: 'root'
})
export class SendNotificationHttpService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

    getCurrentIndianTime(): Date {
        const istOffsetMinutes = 5 * 60 + 30;
        const utcNow = new Date();
        utcNow.setMinutes(utcNow.getMinutes() + istOffsetMinutes);
        return utcNow;
    }

    sendNotification(): Observable<GetNotificationResponse[]> {
        const indianTime = this.getCurrentIndianTime();

        const dateFrom = new Date(indianTime);
        dateFrom.setMinutes(dateFrom.getMinutes() - 10);

        const dateTo = new Date(indianTime);

        const requestData: ISendNotification = {
            dateFrom: dateFrom.toISOString(),
            dateTo: dateTo.toISOString(),
        };

        return this.http.post<GetNotificationResponse[]>(this.baseUrl + "Lotto/CheckForNumberNoti", requestData);
    }
}
