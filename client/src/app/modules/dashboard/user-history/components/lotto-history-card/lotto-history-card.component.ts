import { Component, Input } from '@angular/core';
import { LottoHistory } from '../lotto-history/models/lottoHistory';

@Component({
  selector: 'app-lotto-history-card',
  templateUrl: './lotto-history-card.component.html',
  styleUrls: ['./lotto-history-card.component.scss']
})
export class LottoHistoryCardComponent {
  @Input() lotto!: LottoHistory;
}
