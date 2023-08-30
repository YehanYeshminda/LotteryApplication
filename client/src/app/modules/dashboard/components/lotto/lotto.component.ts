import { Component, OnInit } from '@angular/core';
import { CompanyCode, LottoHttpService } from "./services/lotto-http.service";
import { debounceTime, Observable, of, tap } from "rxjs";
import { GetLotto } from "./models/lotto";
import { errorNotification, successNotification } from "@shared/alerts/sweetalert";
import { getAuthDetails } from "@shared/methods/methods";
import { CookieService } from "ngx-cookie-service";
import { CartEntityService } from "../cart/services/cart-entity.service";
import { GetMegaDrawHistory } from '../easy-draw/models/EasyDrawResponse';

@Component({
  selector: 'app-lotto',
  templateUrl: './lotto.component.html',
  styleUrls: ['./lotto.component.scss']
})
export class LottoComponent implements OnInit {
  lottoNo$: Observable<GetLotto> = of();
  selectedCompany: string = "";
  numbers: number[] = Array.from({ length: 10 }, (_, i) => (i + 1) % 10);
  selectedNumber: number | null = null;
  companyCode$: Observable<CompanyCode> = of();
  selectedCode$: Observable<string> = of();
  numValue: string = ""
  lottoDrawTime!: Date; // The time of the next Mega Draw
  remainingTime!: string;
  lottoDrawHistory$: Observable<GetMegaDrawHistory[]> = of([]);

  constructor(private lottoHttpService: LottoHttpService, private cookieService: CookieService, private cartEntityService: CartEntityService) { }

  ngOnInit(): void {
    this.lottoNo$ = this.lottoHttpService.getLottoNo();
    this.lottoDrawHistory$ = this.lottoHttpService.getLottoDrawPastDayHistory();
    this.companyCode$ = this.lottoHttpService.getCompanyCode().pipe(
      tap((response) => {
        this.numValue = response.companyCode + "-01";
      })
    )

    this.loadLottoDrawTime();
    setInterval(() => {
      this.updateRemainingTime();
    }, 1000);

    setInterval(() => {
      this.loadLottoDrawTime();
    }, 3000)

    setInterval(() => {
      this.lottoDrawHistory$ = this.lottoHttpService.getLottoDrawPastDayHistory();
    }, 60000)
  }

  resetSelection(): void {
    this.selectedNumber = null;
  }

  loadLottoDrawTime(): void {
    this.lottoHttpService.getLottoDrawRemainingTime().subscribe(
      (nextExecutionTime: Date) => {
        this.lottoDrawTime = nextExecutionTime;
        this.updateRemainingTime();
      },
      (error) => {
        console.error('Error loading Mega Draw time:', error);
      }
    );
  }

  updateRemainingTime(): void {
    if (!this.lottoDrawTime) {
      return; // Ensure megaDrawTime is set before calculating remaining time
    }

    const currentTime = new Date().getTime();
    const endTime = new Date(this.lottoDrawTime).getTime();
    const timeDiff = endTime - currentTime;

    const seconds = Math.floor((timeDiff / 1000) % 60);
    const minutes = Math.floor((timeDiff / 1000 / 60) % 60);
    const hours = Math.floor(timeDiff / 1000 / 3600);

    this.remainingTime = `${hours}:${minutes}:${seconds}`;
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
}
