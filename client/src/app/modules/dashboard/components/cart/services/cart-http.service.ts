import { Injectable } from '@angular/core';
import { Cart, CartReponse } from "../models/cart";
import { Observable, of } from "rxjs";
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { AuthDetails } from 'src/app/shared/models/auth';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';

export interface DeleteAllFromCartResponse {
  packageId: string
}

@Injectable({
  providedIn: 'root'
})
export class CartHttpService {
  private cartItems: Cart[] = [];
  baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient, private cookieService: CookieService) {
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

  removeAllFromCart(): Observable<DeleteAllFromCartResponse> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));
    return this.http.post<DeleteAllFromCartResponse>(this.baseUrl + "Cart/DeleteAllFromCart", auth)
  }
}
