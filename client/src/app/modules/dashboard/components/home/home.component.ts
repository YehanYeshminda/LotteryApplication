import { Component, OnInit } from '@angular/core';
import { HomeEntityService } from './services/home-entity.service';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppState } from 'src/app/reducer';
import { Store } from '@ngrx/store';
import { OldRafflesReponse } from './models/home';
import { selectDrawHistoryData, selectDrawHistoryEasyData, selectDrawHistoryMegaData } from './features/drawHistory.selectors';
import { splitNumbersByTwo } from 'src/app/shared/methods/methods';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  drawNo$: Observable<string[] | null> = of();
  drawCount$: Observable<string | undefined> = of();
  drawDate$: Observable<string | undefined> = of();
  megaDrawNo$: Observable<string[] | null> = of();
  megaDrawCount$: Observable<string | undefined> = of();
  megaDrawDate$: Observable<string | undefined> = of();
  megaDrawuniqueNo$: Observable<string | undefined> = of();
  easyDrawuniqueNo$: Observable<string | undefined> = of();
  interval = 2000;
  currentDate = new Date();
  megaDrawHistory$: Observable<OldRafflesReponse[] | undefined> = of([]);
  easyDrawHistory$: Observable<OldRafflesReponse[] | undefined> = of([]);

  constructor(private homeEntityService: HomeEntityService, private store: Store) { }

  ngOnInit(): void {
    this.drawNo$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const easyDraw = response.find((draws) => draws.raffleName === 'EasyDraw');
        return easyDraw?.ticketNo?.toString()?.match(/.{1,2}/g) ?? null;
      })
    );

    this.megaDrawNo$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const megaDraw = response.find((draws) => draws.raffleName === 'MegaDraw');
        return megaDraw?.ticketNo?.toString()?.match(/.{1,2}/g) ?? null;
      })
    );

    this.drawCount$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const easyDraw = response.find((draws) => draws.raffleName === 'EasyDraw');
        return easyDraw?.drawCount?.toString();
      })
    );

    this.megaDrawCount$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const megaDraw = response.find((draws) => draws.raffleName === 'MegaDraw');
        return megaDraw?.drawCount?.toString();
      })
    );

    this.drawDate$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const easyDraw = response.find((draws) => draws.raffleName === 'EasyDraw');
        return easyDraw?.startOn;
      })
    );

    this.megaDrawDate$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const megaDraw = response.find((draws) => draws.raffleName === 'MegaDraw');
        return megaDraw?.startOn;
      })
    );

    this.megaDrawuniqueNo$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const megaDraw = response.find((draws) => draws.raffleName === 'MegaDraw');
        return megaDraw?.uniqueRaffleId?.toString();
      }));

    this.easyDrawuniqueNo$ = this.homeEntityService.entities$.pipe(
      map(response => {
        const easyDraw = response.find((draws) => draws.raffleName === 'EasyDraw');
        return easyDraw?.uniqueRaffleId?.toString();
      }));

    this.megaDrawHistory$ = this.store.select(selectDrawHistoryMegaData);
    this.easyDrawHistory$ = this.store.select(selectDrawHistoryEasyData);

    this.megaDrawHistory$.subscribe({
      next: response => {
        console.log(response);
      }
    })
  }

  splitValuesAndreturnArray(value: string | undefined): string[] {
    return splitNumbersByTwo(value ?? '');
  }
}
