import { Component, OnInit } from '@angular/core';
import { EasyDrawHttpService } from './services/easy-draw-http.service';
import { BuyEasyDraw, EasyDrawResponse, FullEasyDraw, GetMegaDrawHistory } from './models/EasyDrawResponse';
import { CookieService } from "ngx-cookie-service";
import { errorNotification, successNotification } from 'src/app/shared/alerts/sweetalert';
import { getAuthDetails } from '@shared/methods/methods';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-easy-draw',
  templateUrl: './easy-draw.component.html',
  styleUrls: ['./easy-draw.component.scss']
})
export class EasyDrawComponent implements OnInit {
  latestNumbers: number[] = [];
  drawExecuted = false;
  megaDrawTime!: Date; // The time of the next Mega Draw
  remainingTime!: string;
  easyDraw$: Observable<FullEasyDraw> = of();
  easyDrawHistory$: Observable<GetMegaDrawHistory[]> = of([]);

  constructor(private easyDrawHttpService: EasyDrawHttpService, private cookieService: CookieService) { }

  ngOnInit(): void {
    this.easyDraw$ = this.easyDrawHttpService.getEasyDraw();
    this.easyDrawHistory$ = this.easyDrawHttpService.getEasyDrawPastDayHistory();

    this.loadMegaDrawTime();
    setInterval(() => {
      this.updateRemainingTime()
    }, 1000);

    setInterval(() => {
      this.loadMegaDrawTime();
    }, 5000)
  }

  drawRandomNumber() {
    const authDetails = getAuthDetails(this.cookieService.get('user'));

    if (authDetails) {
      this.easyDrawHttpService.getEasyDrawNumbers(authDetails).subscribe((response: EasyDrawResponse) => {
        this.latestNumbers = response.result;
        this.drawExecuted = true;
      });
    }
  }

  loadMegaDrawTime(): void {
    this.easyDrawHttpService.getEasyDrawRemainingTime().subscribe(
      (nextExecutionTime: Date) => {
        this.megaDrawTime = nextExecutionTime;
        console.log(nextExecutionTime);
        this.updateRemainingTime(); // Update remaining time after setting megaDrawTime
      },
      (error) => {
        console.error('Error loading Mega Draw time:', error);
      }
    );
  }

  updateRemainingTime(): void {
    if (!this.megaDrawTime) {
      return; // Ensure megaDrawTime is set before calculating remaining time
    }

    const currentTime = new Date().getTime();
    const endTime = new Date(this.megaDrawTime).getTime();
    const timeDiff = endTime - currentTime;

    const seconds = Math.floor((timeDiff / 1000) % 60);
    const minutes = Math.floor((timeDiff / 1000 / 60) % 60);
    const hours = Math.floor(timeDiff / 1000 / 3600);

    this.remainingTime = `${hours}:${minutes}:${seconds}`;
  }


  buyEasyDraw() {
    if (getAuthDetails(this.cookieService.get('user')) != null) {
      const newEasyDraw: BuyEasyDraw = {
        authDto: getAuthDetails(this.cookieService.get('user')),
        raffleId: "2",
        ticketNo: this.latestNumbers.join(""),
      }
      this.easyDrawHttpService.buyEasyDrawNo(newEasyDraw).subscribe({
        next: response => {
          if (response.isSuccess) {
            successNotification(`Successfully bought ${response.result.ticketNo} with order reference number of ${response.result.lotteryReferenceId}!`);
            this.drawRandomNumber();
          } else {
            errorNotification(response.message);
          }
        },
      });
    }
    else {
      errorNotification('Please login to add to cart');
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
}
