import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, switchMap, take } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/reducer';
import { SingleUserHttpService } from '../services/single-user-http.service';
import { UserInfoActions } from '../features/user-info.types';
import { selectSingleUserInfoLoaded } from '../features/user-info.selectors';

@Injectable()
export class SingleUserInfoResolver implements Resolve<boolean> {
  constructor(private singleUserHttpService: SingleUserHttpService, private store: Store<AppState>) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.store.select(selectSingleUserInfoLoaded).pipe(
      take(1),
      switchMap((isLoaded) => {
        if (isLoaded) {
          return of(true);
        } else {
          return this.singleUserHttpService.getSingleUserInfo().pipe(
            map((data) => {
              this.store.dispatch(UserInfoActions.SaveSingleUserInfo({ singleUserInfo: data }));
              return true;
            }),
            catchError((error) => {
              console.error('Error loading user history:', error);
              return of(false);
            })
          );
        }
      })
    );
  }
}
