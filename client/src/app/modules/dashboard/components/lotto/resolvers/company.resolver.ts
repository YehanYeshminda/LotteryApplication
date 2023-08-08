import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, catchError, map, of, switchMap, take } from 'rxjs';
import { CompanyHttpService } from '../services/company-http.service';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/reducer';
import { selectCompanyLoaded } from '../features/company.selector';
import { CompanyActions } from '../features/company.types';

@Injectable({
  providedIn: 'root'
})
export class CompanyResolver implements Resolve<boolean> {
  constructor(private companyHttpService: CompanyHttpService, private store: Store<AppState>) { }
  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.store.select(selectCompanyLoaded).pipe(
      take(1),
      switchMap((isLoaded) => {
        if (isLoaded) {
          return of(true);
        } else {
          return this.companyHttpService.getCompany().pipe(
            map((data) => {
              this.store.dispatch(CompanyActions.SaveCompany({ company: data }));
              return true;
            }),
            catchError((error) => {
              console.error('Error loading draw history:', error);
              return of(false);
            })
          );
        }
      })
    );
  }
}
