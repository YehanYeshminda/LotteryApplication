import { createFeatureSelector, createSelector } from "@ngrx/store";
import { UserHistoryState } from "./history.reducer";

export const selectUserHistoryState = createFeatureSelector<UserHistoryState>('userHistory');
export const selectUserHistoryWinState = createFeatureSelector<UserHistoryState>('userWinHistory');

export const selectUserHistoryDataLoaded = createSelector(
    selectUserHistoryState,
    (state: UserHistoryState) => state.isLoaded
);

export const selectUserHistoryData = createSelector(
    selectUserHistoryState,
    (state: UserHistoryState) => state.userHistory
);

export const selectUserHistoryIsWin = createSelector(
    selectUserHistoryWinState,
    (state: UserHistoryState) => {
        if (state.userHistory) {
            return state.userHistory
                .filter((userHistory) => userHistory.isWin)
        }
        return [];
    }
)