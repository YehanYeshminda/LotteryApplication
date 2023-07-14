import { createReducer, on } from '@ngrx/store';
import { CartReponse } from '../models/cart';
import { clearCartEntities } from './cart.action';

export interface CartState {
    ids: number[];
    entities: { [key: number]: CartReponse };
    entityName: string;
    filter: string;
    loaded: boolean;
    loading: boolean;
    changeState: any;
}

export const initialState: CartState = {
    ids: [],
    entities: {},
    entityName: 'Cart',
    filter: '',
    loaded: false,
    loading: false,
    changeState: {},
};

const cartReducer = createReducer(
    initialState,
    on(clearCartEntities, () => initialState)
);

export function reducer(state: any, action: any) {
    return cartReducer(state, action);
}
