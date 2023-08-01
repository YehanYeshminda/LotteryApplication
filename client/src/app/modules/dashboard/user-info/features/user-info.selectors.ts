import { createFeatureSelector, createSelector } from "@ngrx/store";
import { SingleUserInfoState } from "./user-info.reducer";

export const selectSingleUserHistoryState = createFeatureSelector<SingleUserInfoState>('singleUserInfo');

export const selectSingleUserInfoLoaded = createSelector(
    selectSingleUserHistoryState,
    (state: SingleUserInfoState) => state.isLoaded
);

export const selectSingleUserInfo = createSelector(
    selectSingleUserHistoryState,
    (state: SingleUserInfoState) => state.singleUserInfo
);