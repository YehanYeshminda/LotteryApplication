import { createAction, props } from "@ngrx/store";
import { OldRafflesReponse } from "../models/home";

export const SaveDrawHistory = createAction(
    '[History Page] History Save All',
    props<{ drawHistory: OldRafflesReponse }>()
);