import { Component, OnInit } from '@angular/core';
import { EasyDrawHttpService } from './services/easy-draw-http.service';
import { BuyEasyDraw, EasyDrawResponse, FullEasyDraw } from './models/EasyDrawResponse';
import { CookieService } from "ngx-cookie-service";
import { confirmApproveNotification, errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { getAuthDetails } from '@shared/methods/methods';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-easy-draw',
  templateUrl: './easy-draw.component.html',
  styleUrls: ['./easy-draw.component.scss']
})
export class EasyDrawComponent implements OnInit {
  latestNumbers: number[] = [];
  drawExecuted = false;
  easyDraw$: Observable<FullEasyDraw> = of();

  constructor(private easyDrawHttpService: EasyDrawHttpService, private cookieService: CookieService) { }

  ngOnInit(): void {
    this.drawRandomNumber();
    this.easyDraw$ = this.easyDrawHttpService.getEasyDraw();
  }

  drawRandomNumber() {
    const authDetails = getAuthDetails(this.cookieService.get('user'));

    if (authDetails) {
      this.easyDrawHttpService.getEasyDrawNumbers(authDetails).subscribe((response: EasyDrawResponse) => {
        this.latestNumbers = response.result;
        this.drawExecuted = true;
      });
    }
  }

  buyEasyDraw() {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      const newEasyDraw: BuyEasyDraw = {
        authDto: getAuthDetails(this.cookieService.get('user')),
        raffleId: "2",
        ticketNo: this.latestNumbers.join(""),
      }

      confirmApproveNotification('Are you sure you want to buy this ticket?').then((result) => {
        if (result.isConfirmed) {
          this.easyDrawHttpService.buyEasyDrawNo(newEasyDraw).subscribe({
            next: response => {
              if (response.isSuccess) {
                successNotification(`Successfully bought ${response.result.ticketNo} with order reference number of ${response.result.lotteryReferenceId}!`);
                this.drawRandomNumber();
              } else {
                errorNotification(response.message);
              }
            },
          });
        }
      })

    } else {
      errorNotification('Please login to add to cart');
    }
  }

  getUserTimeZone() {
    try {
      if ('Intl' in window && 'DateTimeFormat' in Intl) {
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        return timeZone;
      } else {
        const offset = new Date().getTimezoneOffset();
        const positiveOffset = Math.abs(offset);
        const hours = String(Math.floor(positiveOffset / 60)).padStart(2, '0');
        const minutes = String(positiveOffset % 60).padStart(2, '0');
        const sign = offset > 0 ? '-' : '+';
        return `${sign}${hours}:${minutes}`;
      }
    } catch (error) {
      console.error('Error while getting the user time zone:', error);
      return 'UTC';
    }
  }
}
