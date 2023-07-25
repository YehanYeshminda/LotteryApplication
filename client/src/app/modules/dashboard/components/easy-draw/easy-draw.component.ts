import { Component } from '@angular/core';
import { EasyDrawHttpService } from './services/easy-draw-http.service';
import { EasyDrawResponse } from './models/EasyDrawResponse';
import { CookieService } from "ngx-cookie-service";
import { getAuthDetails } from "../../../../shared/methods/methods";
import { errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { CartEntityService } from '../cart/services/cart-entity.service';

@Component({
  selector: 'app-easy-draw',
  templateUrl: './easy-draw.component.html',
  styleUrls: ['./easy-draw.component.scss']
})
export class EasyDrawComponent {
  latestNumbers: number[] = [];
  drawExecuted = false;

  constructor(
    private easyDrawHttpService: EasyDrawHttpService,
    private cartEntityService: CartEntityService,
    private cookieService: CookieService) { }

  drawRandomNumber() {
    const authDetails = getAuthDetails(this.cookieService.get('user'));

    if (authDetails) {
      this.easyDrawHttpService.getEasyDrawNumbers(authDetails).subscribe((response: EasyDrawResponse) => {
        this.latestNumbers = response.result;
        console.log(this.latestNumbers);
        this.drawExecuted = true;
      });
    }
  }

  addToCart() {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      const newCartItem = {
        cartNumbers: this.latestNumbers,
        paid: 500,
        name: "Easy Draw",
        addOn: new Date().toISOString(),
        authDto: getAuthDetails(this.cookieService.get('user')),
        price: 500,
        raffleId: "2",
        lotteryStatus: 0,
        raffleNo: "",
        userId: 0
      };

      this.cartEntityService.add(newCartItem).subscribe({
        next: response => {
          successNotification(response.lotteryNo + " has been successfully added to the cart!");
          this.drawRandomNumber();
        }, error: error => {
          errorNotification("Lottery number already inside of cart!");
        }
      });
    } else {
      errorNotification('Please login to add to cart');
    }
  }
}
