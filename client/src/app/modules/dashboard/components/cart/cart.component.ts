import { Component, OnInit } from '@angular/core';
import { CartReponse } from "./models/cart";
import { Observable, map, of, tap } from "rxjs";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { confirmDeleteNotification, errorNotification } from 'src/app/shared/alerts/sweetalert';
import { CartEntityService } from './services/cart-entity.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cartItems$: Observable<CartReponse[]> = of([]);
  total = 0;

  constructor(private cookieService: CookieService, private cartEntityService: CartEntityService) { }

  removeCartItem(item: CartReponse): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      confirmDeleteNotification('Are you sure you want to remove from cart?').then((result) => {
        if (result.isConfirmed) {
          const itemId: string | number = item.id || '';
          this.cartEntityService.delete(itemId).subscribe({
            next: (response) => {
            },
            error: (error) => {
              console.error(error);
            }
          });
        }
      })
    } else {
      errorNotification('Please login to remove from cart');
    }
  }

  ngOnInit(): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems$ = this.cartEntityService.entities$.pipe(
        tap(response => {
          this.total = 0;
        }),
        map(response => {
          this.total = response.reduce((acc, item) => acc + item.paid, 0);
          return response;
        })
      );
    }
  }
}
