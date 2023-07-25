import { createAction, props } from "@ngrx/store";
import { UserHistoryResponse } from "../models/userhistory";

export const SaveUserHistory = createAction(
    '[UserHistory Page] User History Save All',
    props<{ userHistory: UserHistoryResponse }>()
);