import {Component, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import {Store} from "@ngrx/store";
import {AppState} from "../../reducer";
import {logout} from "../../modules/dashboard/auth/features/auth.actions";
import {CartHttpService} from "../../modules/dashboard/components/cart/services/cart-http.service";
import {Observable, of} from "rxjs";
import {Cart} from "../../modules/dashboard/components/cart/models/cart";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  cartItemsCount$: Observable<Cart[]> = of([]);
  constructor(private router: Router, private store: Store<AppState>, private cartHttpService: CartHttpService) {}

  logOut() {
    this.store.dispatch(logout());
  }

  ngOnInit(): void {
    this.cartItemsCount$ = this.cartHttpService.getCartItems();
  }
}
