import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Store } from '@ngrx/store';
import { BehaviorSubject, Observable, catchError, map, of, switchMap, take } from 'rxjs';
import { AppState } from 'src/app/reducer';
import { selectUserHistoryDataLoaded } from '../features/history.selector';
import { UserHistoryHttpService } from '../services/user-history-http.service';
import { UserHistoryActions } from '../features/history.types';
import { AuthDetails } from 'src/app/shared/models/auth';
import { getAuthDetails } from 'src/app/shared/methods/methods';
import { CookieService } from 'ngx-cookie-service';
import { PagedList } from '../models/userhistory';

@Injectable()
export class UserHistoryResolver implements Resolve<boolean> {
  constructor(private userHistoryHttpService: UserHistoryHttpService, private store: Store<AppState>, private cookieService: CookieService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const auth: AuthDetails | null = getAuthDetails(this.cookieService.get('user'));

    if (!auth) return of(false);
    const values: PagedList = {
      authDto: auth,
      pageNumber: 1,
      pageSize: 12
    }

    return this.store.select(selectUserHistoryDataLoaded).pipe(
      take(1),
      switchMap((isLoaded) => {
        // if (isLoaded) {
        //   return of(true);
        // } else {
        return this.userHistoryHttpService.getUserDrawHistoryById(values).pipe(
          map((data) => {
            this.store.dispatch(UserHistoryActions.SaveUserHistory({ userHistory: data }));
            return true;
          }),
          catchError((error) => {
            console.error('Error loading draw history:', error);
            return of(false);
          })
        );
        // }
      })
    );
  }
}
