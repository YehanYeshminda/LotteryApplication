import {Component, OnInit} from '@angular/core';
import {TournamentHttpService} from "../../services/tournament-http.service";
import {Observable, of} from "rxjs";
import {TournamentDrawsInterface} from "../../models/tournament";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  tournamentDraws$: Observable<TournamentDrawsInterface[]> = of([]);
  constructor(private tournamentHttpService: TournamentHttpService) {}

  ngOnInit(): void {
    this.tournamentDraws$ = this.tournamentHttpService.getAllDraws();
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
