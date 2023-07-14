import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, filter, first, of, tap } from 'rxjs';
import { HomeEntityService } from '../services/home-entity.service';

@Injectable()
export class SequenceNoResolver implements Resolve<boolean> {
  constructor(private homeEntityService: HomeEntityService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.homeEntityService.loaded$
      .pipe(
        tap(loaded => {
          if (!loaded) {
            this.homeEntityService.getAll();
          }
        }),
        filter(loaded => !!loaded),
        first()
      )
  }
}
