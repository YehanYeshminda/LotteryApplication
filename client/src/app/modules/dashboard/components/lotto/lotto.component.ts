import { Component, OnInit } from '@angular/core';
import { LottoHttpService } from "./services/lotto-http.service";
import { debounceTime, Observable, of } from "rxjs";
import { GetLotto } from "./models/lotto";
import { confirmApproveNotification, errorNotification, successNotification } from "@shared/alerts/sweetalert";
import { getAuthDetails } from "@shared/methods/methods";
import { CookieService } from "ngx-cookie-service";
import { CartEntityService } from "../cart/services/cart-entity.service";

@Component({
  selector: 'app-lotto',
  templateUrl: './lotto.component.html',
  styleUrls: ['./lotto.component.scss']
})
export class LottoComponent implements OnInit {
  lottoNo$: Observable<GetLotto> = of();
  selectedCompany: string = "";

  constructor(private lottoHttpService: LottoHttpService, private cookieService: CookieService, private cartEntityService: CartEntityService) { }

  ngOnInit(): void {
    this.lottoNo$ = this.lottoHttpService.getLottoNo();
  }

  getLottoNumber() {
    this.lottoNo$ = this.lottoHttpService.getLottoNo().pipe(
      debounceTime(5000)
    );
  }

  buyNewLotto(lotto: GetLotto) {
    confirmApproveNotification(`Are you sure you want to buy this Lotto?`).then(response => {
      if (response.isConfirmed) {
        if (getAuthDetails(this.cookieService.get('user')) != null) {
          const dynamicValue = lotto.lottoNo.substring(0, 2);

          const newCartItem = {
            cartNumbers: [],
            paid: 0,
            name: "Lotto",
            addOn: new Date().toISOString(),
            authDto: getAuthDetails(this.cookieService.get('user')),
            price: 0,
            raffleId: dynamicValue,
            lotteryStatus: 2,
            raffleNo: lotto.lottoNo,
            userId: 0,
            type: "Lotto"
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
    })
  }
}
