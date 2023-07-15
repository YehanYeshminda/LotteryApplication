import { Component, OnInit } from '@angular/core';
import { Observable, map, of, take } from "rxjs";
import { MegaDrawResponse } from "./models/megaDraw";
import { MegaDrawHttpService } from './services/mega-draw-http.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-mega-draw',
  templateUrl: './mega-draw.component.html',
  styleUrls: ['./mega-draw.component.scss']
})
export class MegaDrawComponent implements OnInit {
  drawNumbers$: Observable<number[]> = of([]);
  selectedItems$: Observable<number[]> = of([]);
  constructor(private route: ActivatedRoute) { }

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

  makeMegaDraw() {
    this.selectedItems$.subscribe({
      next: (items) => {
        console.log(items)
      }
    })
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
