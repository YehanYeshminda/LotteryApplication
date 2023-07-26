import { Component, Input } from '@angular/core';
import { UserHistoryResponse } from '../../models/userhistory';

@Component({
  selector: 'app-historycard',
  templateUrl: './historycard.component.html',
  styleUrls: ['./historycard.component.scss']
})
export class HistorycardComponent {
  @Input() userHistoryData!: UserHistoryResponse;

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
