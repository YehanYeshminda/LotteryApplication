import { Injectable } from '@angular/core';
import {Cart} from "../models/cart";
import {Observable, of} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class CartHttpService {
  private cartItems: Cart[] = [];

  constructor() {
    const storedCartItems = localStorage.getItem('cartItems');
    if (storedCartItems) {
      this.cartItems = JSON.parse(storedCartItems);
    }
  }

  addToCart(item: Cart): void {
    this.cartItems.push(item);
    this.saveCartItems();
  }

  getCartItems(): Observable<Cart[]> {
    return of(this.cartItems);
  }

  clearCart(): void {
    this.cartItems = [];
    this.saveCartItems();
  }

  removeFromCart(item: any): void {
    const index = this.cartItems.indexOf(item);
    if (index > -1) {
      this.cartItems.splice(index, 1);
      this.saveCartItems();
    }
  }

  private saveCartItems(): void {
    localStorage.setItem('cartItems', JSON.stringify(this.cartItems));
  }
}
