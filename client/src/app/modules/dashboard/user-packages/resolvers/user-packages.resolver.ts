import { Injectable } from '@angular/core';
import {
  Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import {catchError, map, Observable, of, switchMap, take} from 'rxjs';
import {UserPackageHttpService} from "../services/user-package-http.service";
import {Store} from "@ngrx/store";
import {AppState} from "../../../../reducer";
import {selectUserPackageDataLoaded} from "../features/user-packages.selectors";
import {UserPackagesActions} from "../features/user-packages.types";

@Injectable({
  providedIn: 'root'
})
export class UserPackagesResolver implements Resolve<boolean> {
  constructor(private userPackageHttpService: UserPackageHttpService, private store: Store<AppState>) {}
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.store.select(selectUserPackageDataLoaded).pipe(
      take(1),
      switchMap((isLoaded) => {
        if (isLoaded) {
          return of(true);
        } else {
          return this.userPackageHttpService.getAllPackagesForUser().pipe(
            map((data) => {
              this.store.dispatch(UserPackagesActions.SaveUserPackageHistory({userPackages: data}));
              return true;
            }),
            catchError((error) => {
              console.error('Error loading user packages:', error);
              return of(false);
            })
          );
        }
      }
    ))
  }
}
