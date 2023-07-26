import { Component, OnInit } from '@angular/core';
import { Observable, map, of, take, tap } from 'rxjs';
import { PagedList, UserHistoryResponse } from '../../models/userhistory';
import { Store } from '@ngrx/store';
import { selectUserHistoryData, selectUserHistoryDataLoaded, selectUserHistoryIsWin } from '../../features/history.selector';
import { UserHistoryHttpService } from '../../services/user-history-http.service';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { AuthDetails } from 'src/app/shared/models/auth';
import { CookieService } from 'ngx-cookie-service';
import { UserHistoryActions } from '../../features/history.types';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class HistoryComponent implements OnInit {
  userHistoryData$: Observable<UserHistoryResponse[] | undefined> = of([]);
  userHistoryDataWin$: Observable<UserHistoryResponse[] | undefined> = of([]);
  userHistoryBasedOnLast3Days$: Observable<UserHistoryResponse[] | undefined> = of([]);
  noUserHistory$: Observable<boolean> | undefined = of(true);
  noWinners$: Observable<boolean> | undefined = of(true);
  loading$: Observable<boolean> | undefined = of(true);
  pageSize = 12;
  canLoadMore = true;
  loading = false;

  constructor(private store: Store, private userHistoryHttpService: UserHistoryHttpService, private cookieService: CookieService, private spinnerService: NgxSpinnerService) { }

  ngOnInit(): void {
    this.userHistoryData$ = this.store.select(selectUserHistoryData).pipe(
      tap((data) => {
        if (data) {
          if (data.length === 0) {
            this.noUserHistory$ = of(false);
          }
        }
      })
    );

    this.userHistoryDataWin$ = this.userHistoryHttpService.getUserDrawHistoryWinnings();
    this.userHistoryBasedOnLast3Days$ = this.userHistoryHttpService.getUserDrawHistoryForLast3Days();
  }

  onScrollDown() {
    if (!this.canLoadMore) {
      return;
    }

    this.pageSize += 12;

    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));

    if (!auth) return;
    const values: PagedList = {
      authDto: auth,
      pageNumber: 1,
      pageSize: this.pageSize
    };

    this.canLoadMore = false;
    this.loading = true;

    this.userHistoryHttpService.getUserDrawHistoryById(values).pipe(
      take(1),
      map((data) => {
        console.log
        let exitingHistoryData: UserHistoryResponse[] | undefined = []
        this.store.select(selectUserHistoryData).subscribe({
          next: response => {
            exitingHistoryData = response;
          }
        });

        this.store.dispatch(UserHistoryActions.SaveUserHistoryToExistingData({ newUserHistory: data }));
      })
    ).subscribe()



    setTimeout(() => {
      this.canLoadMore = true
      this.loading = false;
    }, 3000);
  }

  onUp() {
    // console.log("scrolled up!");
  }
}
