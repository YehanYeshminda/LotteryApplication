import {Component, OnInit} from '@angular/core';
import {LottoHttpService} from "./services/lotto-http.service";
import {debounceTime, Observable, of} from "rxjs";
import {GetLotto} from "./models/lotto";

@Component({
  selector: 'app-lotto',
  templateUrl: './lotto.component.html',
  styleUrls: ['./lotto.component.scss']
})
export class LottoComponent implements OnInit {
  lottoNo$: Observable<GetLotto> = of();

  constructor(private lottoHttpService: LottoHttpService) {}

  ngOnInit(): void {}

  getLottoNumber() {
    this.lottoNo$ = this.lottoHttpService.getLottoNo().pipe(
        debounceTime(5000)
    );
  }
}
