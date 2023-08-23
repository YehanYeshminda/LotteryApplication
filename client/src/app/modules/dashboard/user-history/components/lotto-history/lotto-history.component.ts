import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { LottoHistory } from './models/lottoHistory';
import { LottoHistoryHttpService } from './services/lotto-history-http.service';

@Component({
  selector: 'app-lotto-history',
  templateUrl: './lotto-history.component.html',
  styleUrls: ['./lotto-history.component.scss']
})
export class LottoHistoryComponent implements OnInit {
  lottoHistory$: Observable<LottoHistory[]> = of([]);

  constructor(private lottoHistoryHttpService: LottoHistoryHttpService) { }

  ngOnInit(): void {
    this.lottoHistory$ = this.lottoHistoryHttpService.getAllLottoHistory();
  }
}
