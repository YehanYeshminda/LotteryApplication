import {Component, OnInit, TemplateRef} from '@angular/core';
import {BsModalService} from "ngx-bootstrap/modal";
import {MegaDrawHttpService} from "./services/mega-draw-http.service";
import {Observable, of, take} from "rxjs";
import {MegaDrawResponse} from "./models/megaDraw";

@Component({
  selector: 'app-mega-draw',
  templateUrl: './mega-draw.component.html',
  styleUrls: ['./mega-draw.component.scss']
})
export class MegaDrawComponent implements OnInit{
  megaDrawNos$: Observable<MegaDrawResponse> = of();
  selectedItems$: Observable<number[]> = of([]);
  constructor(private  megaDrawHttpService: MegaDrawHttpService) { }

  ngOnInit(): void {
    this.megaDrawNos$ = this.megaDrawHttpService.getMegaDraws(31);
  }

  // selectDrawNumber(item: number) {
  //   this.selectedItems$.pipe(take(1)).subscribe((items) => {
  //     if (items.length < 6) {
  //       this.selectedItems$ = of([...items, item]);
  //     }
  //   });
  // }

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
        const newNumbers:any[] = [];

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
