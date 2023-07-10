import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
		const user = sessionStorage.getItem('user');

		if (user) {
			return true;
		} else {
			this.router.navigate(['/']);
			return false;
		}
	}
}
