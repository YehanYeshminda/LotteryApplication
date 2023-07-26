import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, switchMap, take } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { DrawHistoryHttpService } from '../services/draw-history.service';
import { AppState } from 'src/app/reducer';
import { DrawHistoryActions } from '../features/drawHistory.types';
import { selectDrawHistoryLoaded } from '../features/drawHistory.selectors';

@Injectable()
export class DrawHistoryResolver implements Resolve<boolean> {
    constructor(private drawHistoryService: DrawHistoryHttpService, private store: Store<AppState>) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.store.select(selectDrawHistoryLoaded).pipe(
            take(1),
            switchMap((isLoaded) => {
                if (isLoaded) {
                    return of(true);
                } else {
                    return this.drawHistoryService.getAllDrawHistory().pipe(
                        map((data) => {
                            this.store.dispatch(DrawHistoryActions.SaveDrawHistory({ drawHistory: data }));
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
