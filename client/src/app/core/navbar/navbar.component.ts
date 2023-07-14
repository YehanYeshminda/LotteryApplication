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

  constructor(private router: Router, private store: Store<AppState>, private cartEntityService: CartEntityService, private cookieService: CookieService) { }

  logOut() {
    this.store.dispatch(logout());
    this.cartEntityService.clearCache();
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
