import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from "@ngrx/store";
import { AppState } from "../../reducer";
import { logout, clearEntityCache } from "../../modules/dashboard/auth/features/auth.actions";
import { Observable, of } from "rxjs";
import { CartReponse } from "../../modules/dashboard/components/cart/models/cart";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { CartEntityService } from 'src/app/modules/dashboard/components/cart/services/cart-entity.service';
import { clearCartEntities } from 'src/app/modules/dashboard/components/cart/features/cart.action';
import { HomeEntityService } from 'src/app/modules/dashboard/components/home/services/home-entity.service';
import { RestoreInitialState } from 'src/app/modules/dashboard/user-history/features/history.actions';
import { RestoreSingleUserInfoInitialState } from 'src/app/modules/dashboard/user-info/features/user-info.actions';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  isMenuActive = false;
  navbarCollapsed = true;
  cartNo: number = 0;
  cartItems$: Observable<CartReponse[]> = of([]);

  constructor(private store: Store<AppState>, private cartEntityService: CartEntityService, private cookieService: CookieService, private homeEntityService: HomeEntityService) { }

  logOut() {
    this.store.dispatch(logout());
    this.cartEntityService.clearCache();
    this.homeEntityService.clearCache();
    this.store.dispatch(RestoreInitialState());
    this.store.dispatch(RestoreSingleUserInfoInitialState());
  }

  ngOnInit(): void {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems$ = this.cartEntityService.entities$;
    }
  }


  toggleNavbarCollapsing() {
    this.navbarCollapsed = !this.navbarCollapsed;
  }
}
