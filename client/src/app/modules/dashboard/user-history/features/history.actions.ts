import { createAction, props } from "@ngrx/store";
import { UserHistoryResponse } from "../models/userhistory";

export const SaveUserHistory = createAction(
    '[UserHistory Page] User History Save All',
    props<{ userHistory: UserHistoryResponse[] }>()
);

export const SaveUserHistoryToExistingData = createAction(
    '[UserHistory Page] User History Save to existing',
    props<{ newUserHistory: UserHistoryResponse[] }>()
);

export const RestoreInitialState = createAction(
    '[UserHistory Page] User History Clear All'
);
