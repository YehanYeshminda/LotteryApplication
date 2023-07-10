import { createFeatureSelector, createSelector } from '@ngrx/store';
import {AuthState} from "../reducer/auth.reducer";

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const isLoggedIn = createSelector(
	selectAuthState,
	() => {
		const auth = JSON.parse(sessionStorage.getItem('user') || '{}');
		return !!auth;
	}
);

export const isLoggedOut = createSelector(isLoggedIn, (loggedIn) => !loggedIn);
