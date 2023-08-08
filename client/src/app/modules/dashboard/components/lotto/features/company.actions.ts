import { createAction, props } from "@ngrx/store";
import { Company } from "../models/lotto";

export const SaveCompany = createAction(
    '[Lotto Page] Company Save All',
    props<{ company: Company[] }>()
);

export const RestoreInitialState = createAction(
    '[UserHistory Page] Company Clear All'
);
