import { Component, OnInit } from '@angular/core';
import { HomeEntityService } from './services/home-entity.service';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

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
  interval = 2000;

  constructor(private homeEntityService: HomeEntityService) { }

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
  }
}
