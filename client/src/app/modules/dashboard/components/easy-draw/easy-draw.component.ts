import { Component } from '@angular/core';
import { EasyDrawHttpService } from './services/easy-draw-http.service';
import { EasyDrawResponse } from './models/EasyDrawResponse';
import { CartHttpService } from '../cart/services/cart-http.service';
import { Cart } from '../cart/models/cart';
import {CookieService} from "ngx-cookie-service";
import {getAuthDetails} from "../../../../shared/methods/methods";

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
    private cartHttpService: CartHttpService,
    private cookieService: CookieService
  ) {}

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
    let id = 0;

    const newCartItem: Cart = {
      id: id,
      name: 'Easy Draw',
      numbers: this.latestNumbers,
      price: 500
    };

    this.cartHttpService.addToCart(newCartItem);
  }
}
