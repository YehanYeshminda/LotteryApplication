import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RouterReducerState } from '@ngrx/router-store';
import { Observable, of } from 'rxjs';
import { SearchBasedOnHistory, UserHistoryResponse } from '../../user-history/models/userhistory';
import { HistoryHttpService } from '../../services/history-http.service';
import { CookieService } from 'ngx-cookie-service';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { AuthDetails } from 'src/app/shared/models/auth';

@Component({
  selector: 'app-search-history',
  templateUrl: './search-history.component.html',
  styleUrls: ['./search-history.component.scss']
})
export class SearchHistoryComponent implements OnInit {
  searchTerm: string | null = '';
  routerState$: Observable<RouterReducerState<any>> | undefined;
  history$: Observable<UserHistoryResponse[]> = of([]);

  constructor(private route: ActivatedRoute, private historyHttpService: HistoryHttpService, private cookieService: CookieService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.searchTerm = params.get('id');
      this.searchFromSearchTerm();
    });
  }


  searchFromSearchTerm() {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));

    var data: SearchBasedOnHistory = {
      authDto: auth,
      raffleUniqueId: this.searchTerm
    };

    this.history$ = this.historyHttpService.searchForHistories(data);
  }
}
