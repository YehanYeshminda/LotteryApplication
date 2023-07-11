import { Injectable } from '@angular/core';
import {Cart} from "../models/cart";
import {Observable, of} from "rxjs";
import {errorNotification, successNotification} from "../../../../../shared/alerts/sweetalert";

@Injectable({
  providedIn: 'root'
})
export class CartHttpService {
  private cartItems: Cart[] = [];

  constructor() {
    const storedCartItems = sessionStorage.getItem('cartItems');
    if (storedCartItems) {
      this.cartItems = JSON.parse(storedCartItems);
    }
  }

  addToCart(item: Cart): void {
    if (this.cartItems.find(cartItem => cartItem.numbers === item.numbers)) {
      errorNotification('Lottery already in cart!');
      return;
    }

    this.cartItems.push(item);
    successNotification(`Lottery added to cart with ${item.numbers} and ${item.price} price!`);
    this.saveCartItems();
  }

  getCartItems(): Observable<Cart[]> {
    return of(this.cartItems);
  }

  getCartItemById(id: number): Cart | undefined {
    return this.cartItems.find(item => item.id === id);
  }

  removeFromCart(item: any): void {
    const index = this.cartItems.indexOf(item);
    if (index > -1) {
      this.cartItems.splice(index, 1);
      this.saveCartItems();
    }
  }

  clearCart(): void {
    this.cartItems = [];
    this.saveCartItems();
  }

  getTotal(): Observable<number> {
    return of(this.cartItems.reduce((total, item) => total + item.price, 0));
  }

  private saveCartItems(): void {
    sessionStorage.setItem('cartItems', JSON.stringify(this.cartItems));
  }
}
