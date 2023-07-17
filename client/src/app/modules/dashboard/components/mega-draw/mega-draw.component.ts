import { Component, OnInit } from '@angular/core';
import { Observable, map, of, take } from "rxjs";
import { ActivatedRoute } from '@angular/router';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { CartEntityService } from '../cart/services/cart-entity.service';

@Component({
  selector: 'app-mega-draw',
  templateUrl: './mega-draw.component.html',
  styleUrls: ['./mega-draw.component.scss']
})
export class MegaDrawComponent implements OnInit {
  drawNumbers$: Observable<number[]> = of([]);
  selectedItems$: Observable<number[]> = of([]);
  latestNumbers: number[] = [];
  constructor(private route: ActivatedRoute, private cookieService: CookieService, private cartEntityService: CartEntityService) { }

  ngOnInit(): void {
    this.drawNumbers$ = this.route.data.pipe(map(data => data['drawNumbers']));
  }

  selectDrawNumber(item: number) {
    this.selectedItems$.pipe(take(1)).subscribe((items) => {
      if (items.length < 6 && !items.includes(item)) {
        this.selectedItems$ = of([...items, item]);
      }
    });
  }

  addToCart() {
    if (getAuthDetails(this.cookieService.get('user')) != null) {

      this.selectedItems$.subscribe({
        next: response => {
          if (!response) {
            errorNotification("Please select some numbers!");
            return;
          }

          this.latestNumbers = response;
        }
      })


      const newCartItem = {
        cartNumbers: this.latestNumbers,
        paid: 100,
        name: "Mega Draw",
        addOn: new Date().toISOString(),
        authDto: getAuthDetails(this.cookieService.get('user')),
        price: 100,
        raffleId: "2",
        lotteryStatus: 0,
        raffleNo: "",
        userId: 0
      };

      this.cartEntityService.add(newCartItem).subscribe(
        () => {
          successNotification('Added to cart');
        },
        (error) => {
          errorNotification("Lottery number already inside of cart!");
        }
      );
    } else {
      errorNotification('Please login to add to cart');
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
}
