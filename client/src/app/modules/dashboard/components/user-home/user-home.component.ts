import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Observable, interval, of } from "rxjs";
import { TournamentDrawsInterface } from "../../tournaments/models/tournament";
import { TournamentHttpService } from "../../tournaments/services/tournament-http.service";
import { UserWinnerHistoryInfo } from './models/user-home';

@Component({
  selector: 'app-user-home',
  templateUrl: './user-home.component.html',
  styleUrls: ['./user-home.component.scss']
})
export class UserHomeComponent implements OnInit {
  @ViewChild('valueElement') valueElement!: ElementRef;
  animatedValue: number = 0;
  startValue: number = 0;
  endValue: number = 100000;
  duration: number = 5000;
  startTimestamp: number | null = null;
  userWinnerHistory$: Observable<UserWinnerHistoryInfo[]> = of([]);
  today = Date.now();

  tournamentDraws$: Observable<TournamentDrawsInterface[]> = of([]);
  constructor(private tournamentHttpService: TournamentHttpService) { }


  ngOnInit(): void {
    this.animateValue();
    this.fetchUserWinnerHistory();
    this.fetchTournamentInfo();

    interval(10000).subscribe(() => {
      this.fetchUserWinnerHistory();
    });

    interval(60000).subscribe(() => {
      this.fetchTournamentInfo();
    });

    this.tournamentDraws$.subscribe({
      next: response => {
        console.log(response)
      }
    })
  }

  animateValue() {
    const step = (timestamp: number) => {
      if (!this.startTimestamp) {
        this.startTimestamp = timestamp;
      }

      const progress = Math.min((timestamp - this.startTimestamp) / this.duration, 1);
      this.animatedValue = Math.floor(progress * (this.endValue - this.startValue) + this.startValue);

      if (progress < 1) {
        window.requestAnimationFrame(step);
      }
    };

    window.requestAnimationFrame(step);
  }

  fetchTournamentInfo() {
    this.tournamentDraws$ = this.tournamentHttpService.getAllDraws();
  }

  fetchUserWinnerHistory() {
    this.userWinnerHistory$ = this.tournamentHttpService.getAllUserWinnerHistory();
  }
}
