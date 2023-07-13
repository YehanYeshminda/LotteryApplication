import { Component, OnInit, Renderer2 } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from "@ngrx/store";
import { AppState } from "../../reducer";
import { logout } from "../../modules/dashboard/auth/features/auth.actions";
import { CartHttpService } from "../../modules/dashboard/components/cart/services/cart-http.service";
import { Observable, of } from "rxjs";
import { Cart, CartReponse } from "../../modules/dashboard/components/cart/models/cart";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { CartEntityService } from 'src/app/modules/dashboard/components/cart/services/cart-entity.service';

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
