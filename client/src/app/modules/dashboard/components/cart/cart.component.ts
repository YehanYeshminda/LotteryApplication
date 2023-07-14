import { Component, OnInit } from '@angular/core';
import { CartReponse } from "./models/cart";
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
      if (!item) return;
      const itemId: string | number = item.id || '';
      this.cartEntityService.delete(itemId).subscribe({
        next: (response) => {
          console.log(response);
        },
        error: (error) => {
          console.error(error);
        }
      }
      );
    } else {
      errorNotification('Please login to remove from cart');
    }
  }

  ngOnInit(): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems = this.cartEntityService.entities$;
    }
  }

  getTotal(): Observable<number> {
    return of(0);
  }
}
