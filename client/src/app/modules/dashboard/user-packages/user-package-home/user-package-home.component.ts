import { Component, OnInit } from '@angular/core';
import { Store } from "@ngrx/store";
import { AppState } from "../../../../reducer";
import { Observable, of } from "rxjs";
import { UserPackage } from "../models/user-package";
import { getAuthDetails } from "@shared/methods/methods";
import { errorNotification, successNotification } from "@shared/alerts/sweetalert";
import { CookieService } from "ngx-cookie-service";
import { CartEntityService } from "../../components/cart/services/cart-entity.service";
import { UserPackageHttpService } from '../services/user-package-http.service';

@Component({
  selector: 'app-user-package-home',
  templateUrl: './user-package-home.component.html',
  styleUrls: ['./user-package-home.component.scss']
})
export class UserPackageHomeComponent implements OnInit {
  userPackages$: Observable<UserPackage[] | undefined> = of([]);

  constructor(private store: Store<AppState>, private cookieService: CookieService, private cartEntityService: CartEntityService, private userPackageHttpService: UserPackageHttpService) { }

  ngOnInit(): void {
    this.userPackages$ = this.userPackageHttpService.getAllPackagesForUser();
  }

  buyNewPackage(itemId: string, uniqueId: string, packageName: string) {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      const newCartItem = {
        cartNumbers: [],
        paid: 0,
        name: packageName,
        addOn: new Date().toISOString(),
        authDto: getAuthDetails(this.cookieService.get('user')),
        price: 0,
        raffleId: uniqueId,
        lotteryStatus: 1,
        raffleNo: uniqueId,
        userId: 0,
        type: "Package"
      };

      this.cartEntityService.add(newCartItem).subscribe({
        next: response => {
          successNotification("Successfully added to the cart!");
        }
      });
    } else {
      errorNotification('Please login to add to cart');
    }
  }
}
