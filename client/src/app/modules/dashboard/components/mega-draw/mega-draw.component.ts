import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription, map, of, take } from "rxjs";
import { ActivatedRoute } from '@angular/router';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { confirmApproveNotification, errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { FullMegaDraw } from './models/megaDraw';
import { MegaDrawHttpService } from './services/mega-draw-http.service';
import { BuyEasyDraw } from '../easy-draw/models/EasyDrawResponse';
import { EasyDrawHttpService } from '../easy-draw/services/easy-draw-http.service';

@Component({
  selector: 'app-mega-draw',
  templateUrl: './mega-draw.component.html',
  styleUrls: ['./mega-draw.component.scss']
})
export class MegaDrawComponent implements OnInit, OnDestroy {
  drawNumbers$: Observable<number[]> = of([]);
  selectedItems$: Observable<number[]> = of([]);
  latestNumbers: number[] = [];
  megaDrawInfo$: Observable<FullMegaDraw> = of();
  private selectedItemsSubscription!: Subscription;

  constructor(private route: ActivatedRoute, private cookieService: CookieService, private megaDrawHttpService: MegaDrawHttpService, private easyDrawHttpService: EasyDrawHttpService) { }

  ngOnInit(): void {
    this.megaDrawInfo$ = this.megaDrawHttpService.getMegaDraw();
    this.drawNumbers$ = this.route.data.pipe(map(data => data['drawNumbers']));
    this.getRandomDraw();
  }

  selectDrawNumber(item: number) {
    this.selectedItems$.pipe(take(1)).subscribe((items) => {
      if (items.includes(item)) {
        this.selectedItems$ = of(items.filter((num) => num !== item));
      } else if (items.length < 6) {
        this.selectedItems$ = of([...items, item]);
      }
    });
  }


  buyMegaDraw() {
    if (getAuthDetails(this.cookieService.get('user')) != null) {

      this.selectedItemsSubscription = this.selectedItems$.subscribe({
        next: response => {
          const newEasyDraw: BuyEasyDraw = {
            authDto: getAuthDetails(this.cookieService.get('user')),
            raffleId: "1",
            ticketNo: response.join(""),
          }
          this.easyDrawHttpService.buyEasyDrawNo(newEasyDraw).subscribe({
            next: response => {
              if (response.isSuccess) {
                successNotification(`Successfully bought ${response.result.ticketNo} with order reference number of ${response.result.lotteryReferenceId}!`);
                this.getRandomDraw();
              } else {
                errorNotification(response.message);
              }
            },
          });
        }
      })

    } else {
      errorNotification('Please login to add to cart');
    }
  }

  ngOnDestroy(): void {
    if (this.selectedItemsSubscription) {
      this.selectedItemsSubscription.unsubscribe();
    }
  }

  getUserTimeZone() {
    try {
      if ('Intl' in window && 'DateTimeFormat' in Intl) {
        const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
        return timeZone;
      } else {
        const offset = new Date().getTimezoneOffset();
        const positiveOffset = Math.abs(offset);
        const hours = String(Math.floor(positiveOffset / 60)).padStart(2, '0');
        const minutes = String(positiveOffset % 60).padStart(2, '0');
        const sign = offset > 0 ? '-' : '+';
        return `${sign}${hours}:${minutes}`;
      }
    } catch (error) {
      console.error('Error while getting the user time zone:', error);
      return 'UTC';
    }
  }

  getRandomDraw() {
    this.selectedItems$.pipe(take(1)).subscribe({
      next: response => {
        const newNumbers: any[] = [];

        while (newNumbers.length < 6) {
          const randomNumber = Math.floor(Math.random() * 31) + 1;

          if (!newNumbers.includes(randomNumber)) {
            newNumbers.push(randomNumber);
          }
        }

        this.selectedItems$ = of(newNumbers);
      }
    })
  }

  clearAll() {
    this.selectedItems$ = of([]);
  }
}
