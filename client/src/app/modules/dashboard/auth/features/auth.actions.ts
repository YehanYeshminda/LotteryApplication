import { createAction, props } from '@ngrx/store';
import { User } from '../models/user';

export const login = createAction(
	'[Login Page] User Login',
	props<{ user: User }>()
);

export const logout = createAction('[Top Menu] Logout');

export const clearState = createAction('[State] Clear State');

export const clearEntityCache = createAction('[State] Clear Entity Cache');
