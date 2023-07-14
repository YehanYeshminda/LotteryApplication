import { catchError, map, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AddToCartError } from './action';

@Injectable()
export class CartEffects {
    handleAddToCartError$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AddToCartError),
            tap((action) => {
                console.error('Add to cart error:', action.error);
            }),
            map(() => ({ type: 'NOOP_ACTION' })),
            catchError((error) => of({ type: 'HANDLE_ERROR', payload: error }))
        )
    );

    constructor(private actions$: Actions) { }
}
