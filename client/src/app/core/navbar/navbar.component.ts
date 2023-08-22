import { Component, HostListener, OnInit } from '@angular/core';
import { Store } from "@ngrx/store";
import { AppState } from "../../reducer";
import { logout } from "../../modules/dashboard/auth/features/auth.actions";
import { Observable, of } from "rxjs";
import { CartReponse } from "../../modules/dashboard/components/cart/models/cart";
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { CartEntityService } from 'src/app/modules/dashboard/components/cart/services/cart-entity.service';
import { HomeEntityService } from 'src/app/modules/dashboard/components/home/services/home-entity.service';
import { RestoreInitialState } from 'src/app/modules/dashboard/user-history/features/history.actions';
import { RestoreSingleUserInfoInitialState } from 'src/app/modules/dashboard/user-info/features/user-info.actions';
import { AuthDetails } from "../../shared/models/auth";
import { ViewportService } from './services/viewport-service.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  navbarCollapsed = true;
  cartNo: number = 0;
  cartItems$: Observable<CartReponse[]> = of([]);
  isUser$: Observable<boolean> = of(false);
  userDetails: AuthDetails | null = null
  isMenuActive: boolean = false;
  isLessThan768!: boolean;

  toggleMenu() {
    console.log('Toggle menu clicked');
    this.isMenuActive = !this.isMenuActive;
    console.log('isMenuActive:', this.isMenuActive);
  }

  constructor(private store: Store<AppState>, private cartEntityService: CartEntityService, private cookieService: CookieService, private homeEntityService: HomeEntityService, private viewportService: ViewportService) { }

  logOut() {
    this.store.dispatch(logout());
    this.cookieService.delete('user');
    this.cartEntityService.clearCache();
    this.homeEntityService.clearCache();
    this.store.dispatch(RestoreInitialState());
    this.store.dispatch(RestoreSingleUserInfoInitialState());
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.isLessThan768 = event.target.innerWidth < 1000 ? true : false;
  }

  ngOnInit(): void {
    this.isLessThan768 = window.innerWidth < 768 ? true : false;
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      this.cartItems$ = this.cartEntityService.entities$;
      this.isUser$ = of(true);
      this.userDetails = getAuthDetails(this.cookieService.get('user'))
    }
  }


  toggleNavbarCollapsing() {
    this.navbarCollapsed = !this.navbarCollapsed;
  }
}
