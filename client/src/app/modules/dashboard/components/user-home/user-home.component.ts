import {Component, OnInit} from '@angular/core';
import {Observable, of} from "rxjs";
import {TournamentDrawsInterface} from "../../tournaments/models/tournament";
import {TournamentHttpService} from "../../tournaments/services/tournament-http.service";

@Component({
  selector: 'app-user-home',
  templateUrl: './user-home.component.html',
  styleUrls: ['./user-home.component.scss']
})
export class UserHomeComponent implements OnInit {
  tournamentDraws$: Observable<TournamentDrawsInterface[]> = of([]);
  constructor(private tournamentHttpService: TournamentHttpService) {}

  ngOnInit(): void {
    this.tournamentDraws$ = this.tournamentHttpService.getAllDraws();
  }
}
