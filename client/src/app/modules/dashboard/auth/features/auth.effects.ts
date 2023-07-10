import { Injectable } from '@angular/core';
import { AuthActions } from './action-types';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';

@Injectable()
export class AuthEffects {
	login$ = createEffect(
		() =>
			this.actions$.pipe(
				ofType(AuthActions.login),
				tap((action) =>
					sessionStorage.setItem('user', JSON.stringify(action.user))
				)
			),
		{ dispatch: false }
	);

	logout$ = createEffect(
		() =>
			this.actions$.pipe(
				ofType(AuthActions.logout),
				tap((action) => {
					sessionStorage.removeItem('user');
					this.router.navigateByUrl('/products');
				})
			),
		{ dispatch: false }
	);

	constructor(private actions$: Actions, private router: Router) {}
}
