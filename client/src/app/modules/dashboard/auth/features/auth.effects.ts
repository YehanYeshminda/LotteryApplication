import { Injectable } from '@angular/core';
import { AuthActions } from './action-types';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class AuthEffects {
	login$ = createEffect(
		() =>
			this.actions$.pipe(
				ofType(AuthActions.login),
				tap((action) =>
					this.cookieService.set('user', JSON.stringify(action.user), {
            expires: new Date(new Date().getTime() + 1000 * 60 * 24 * 30 * 14),
          })
				)
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

	constructor(private actions$: Actions, private router: Router, private cookieService: CookieService) {}
}
