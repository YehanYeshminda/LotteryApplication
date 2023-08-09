import { Component } from '@angular/core';
import { EasyDrawHttpService } from './services/easy-draw-http.service';
import { BuyEasyDraw, EasyDrawResponse } from './models/EasyDrawResponse';
import { CookieService } from "ngx-cookie-service";
import { errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { getAuthDetails } from '@shared/methods/methods';

@Component({
  selector: 'app-easy-draw',
  templateUrl: './easy-draw.component.html',
  styleUrls: ['./easy-draw.component.scss']
})
export class EasyDrawComponent {
  latestNumbers: number[] = [];
  drawExecuted = false;

  constructor(private easyDrawHttpService: EasyDrawHttpService, private cookieService: CookieService) { }

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

      this.easyDrawHttpService.buyEasyDrawNo(newEasyDraw).subscribe({
        next: response => {
          if (response.isSuccess) {
            successNotification(`Successfully bought ${response.result.ticketNo} with order reference number of ${response.result.lotteryReferenceId}!`);
          } else {
            errorNotification(response.message);
          }
        },
      });

    } else {
      errorNotification('Please login to add to cart');
    }
  }
}
