import { Injectable } from '@angular/core';
import { AuthActions } from './action-types';
import {switchMap, tap} from 'rxjs/operators';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { CookieService } from 'ngx-cookie-service';
import {empty} from "rxjs";

@Injectable()
export class AuthEffects {
	login$ = createEffect(
		() =>
			this.actions$.pipe(
				ofType(AuthActions.login),
				switchMap((action) => {
					const user = JSON.stringify(action.user);
					const existingUser = this.cookieService.get('user');

					if (existingUser) {
						this.cookieService.delete('user');
						console.log(true)
					}

					console.log(false)


					this.cookieService.set('user', user, {
						expires: new Date(new Date().getTime() + 1000 * 60 * 24 * 30 * 14),
					});

					// Return an empty observable as there's no need to dispatch any action
					return empty();
				})
			),
		{ dispatch: false }
	);

	logout$ = createEffect(
		() =>
			this.actions$.pipe(
				ofType(AuthActions.logout),
				tap((action) => {
					this.cookieService.delete('user');
					this.router.navigateByUrl('/');
				})
			),
		{ dispatch: false }
	);

	constructor(private actions$: Actions, private router: Router, private cookieService: CookieService) { }
}
