import { Injectable } from '@angular/core';
import { Cart, CartReponse } from "../models/cart";
import { Observable, of } from "rxjs";
import { errorNotification, successNotification } from "../../../../../shared/alerts/sweetalert";
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { AuthDetails } from 'src/app/shared/models/auth';




@Injectable({
  providedIn: 'root'
})
export class CartHttpService {
  private cartItems: Cart[] = [];
  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {
    const storedCartItems = sessionStorage.getItem('cartItems');
    if (storedCartItems) {
      this.cartItems = JSON.parse(storedCartItems);
    }
  }

  addToCart(item: Cart): Observable<CartReponse> {
    return this.http.post<CartReponse>(this.baseUrl + "Cart/AddToCart", item)
  }

  getCartItems(auth: AuthDetails | null): Observable<CartReponse[]> {
    return this.http.post<CartReponse[]>(this.baseUrl + "Cart/GetCartItems", auth)
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
