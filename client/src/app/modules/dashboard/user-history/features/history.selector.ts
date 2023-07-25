import { createFeatureSelector, createSelector } from "@ngrx/store";
import { UserHistoryState } from "./history.reducer";

export const selectUserHistoryState = createFeatureSelector<UserHistoryState>('drawHistory');

export const selectDrawHistoryLoaded = createSelector(
    selectUserHistoryState,
    (state: UserHistoryState) => state.isLoaded
);

export const selectDrawHistoryData = createSelector(
    selectUserHistoryState,
    (state: UserHistoryState) => state.userHistory
);