import { Component, OnInit } from '@angular/core';
import { CartHttpService } from "./services/cart-http.service";
import { Cart, CartReponse } from "./models/cart";
import { Observable, of } from "rxjs";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { errorNotification } from 'src/app/shared/alerts/sweetalert';
import { CartEntityService } from './services/cart-entity.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cartItems: Observable<CartReponse[]> = of([]);
  total = 0;

  constructor(private cookieService: CookieService, private cartEntityService: CartEntityService) { }

  removeCartItem(item: CartReponse): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      // this.cartService.removeFromCart(item);
      // this.cartItems = this.cartService.getCartItems(getAuthDetails(this.cookieService.get('user')));
    } else {
      errorNotification('Please login to remove from cart');
    }
  }

  // clearCart(): void {
  //   this.cartService.clearCart();
  //   this.cartItems = this.cartService.getCartItems();
  // }

  ngOnInit(): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems = this.cartEntityService.entities$;
    }
  }

  getTotal(): Observable<number> {
    return of(0);
  }
}
