import { Component, OnInit } from '@angular/core';
import { CompanyCode, LottoHttpService } from "./services/lotto-http.service";
import { debounceTime, Observable, of, tap } from "rxjs";
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
  numbers: number[] = Array.from({ length: 10 }, (_, i) => i);
  selectedNumber: number | null = null;
  companyCode$: Observable<CompanyCode> = of();
  selectedCode$: Observable<string> = of();
  numValue: string = ""

  constructor(private lottoHttpService: LottoHttpService, private cookieService: CookieService, private cartEntityService: CartEntityService) { }

  ngOnInit(): void {
    this.lottoNo$ = this.lottoHttpService.getLottoNo();
    this.companyCode$ = this.lottoHttpService.getCompanyCode().pipe(
      tap((response) => {
        this.numValue = response.companyCode + "-01";
      })
    )
  }

  formatNumber(num: number | null, companyCode: string): string {
    if (num === null) {
      return '';
    } else if (num === 0) {
      this.numValue = `${companyCode}-10`;
      return `${companyCode}-10`;
    } else {
      this.numValue = `${companyCode}-${num.toString().padStart(2, '0')}`;
      return `${companyCode}-${num.toString().padStart(2, '0')}`;
    }
  }

  getLottoNumber() {
    this.lottoNo$ = this.lottoHttpService.getLottoNo().pipe(
      debounceTime(5000)
    );
  }

  selectNumber(num: number): void {
    this.selectedNumber = num;
  }

  buyNewLotto() {
    confirmApproveNotification(`Are you sure you want to buy this Lotto?`).then(response => {
      if (response.isConfirmed) {
        if (getAuthDetails(this.cookieService.get('user')) != null) {
          this.lottoHttpService.buyLotto(this.numValue).subscribe(response => {
            if (response.isSuccess) {
              successNotification(response.message);
            }

            if (!response.isSuccess) {
              errorNotification(response.message);
            }
          })
        }
      }
    })
  }
}
