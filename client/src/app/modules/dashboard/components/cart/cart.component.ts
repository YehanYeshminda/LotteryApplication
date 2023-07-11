import {Component, OnInit} from '@angular/core';
import {CartHttpService} from "./services/cart-http.service";
import {Cart} from "./models/cart";
import {Observable, of} from "rxjs";

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cartItems: Observable<Cart[]> = of([]);
  total = 0;

  constructor(private cartService: CartHttpService) {}

  removeCartItem(item: Cart): void {
    this.cartService.removeFromCart(item);
    this.cartItems = this.cartService.getCartItems();
  }

  clearCart(): void {
    this.cartService.clearCart();
    this.cartItems = this.cartService.getCartItems();
  }

  ngOnInit(): void {
    this.cartItems = this.cartService.getCartItems();
  }

  getTotal(): Observable<number> {
    return this.cartService.getTotal();
  }
}
