import { createFeatureSelector, createSelector } from "@ngrx/store";
import { DrawHistoryState } from "./drawHistory.reducer";
import { OldRafflesReponse } from "../models/home";

export const selectDrawHistoryState = createFeatureSelector<DrawHistoryState>('drawHistory');

export const selectDrawHistoryLoaded = createSelector(
    selectDrawHistoryState,
    (state: DrawHistoryState) => state.isLoaded
);

export const selectDrawHistoryData = createSelector(
    selectDrawHistoryState,
    (state: DrawHistoryState) => state.drawHistory
);

export const selectDrawHistoryMegaData = createSelector(
    selectDrawHistoryState,
    (state: DrawHistoryState) => {
        if (state.drawHistory) {
            return state.drawHistory
                .filter((draw: OldRafflesReponse) => draw.lotteryId === 1)
                .map((draw: OldRafflesReponse) => ({ ...draw }));
        }
        return [];
    }
);

export const selectDrawHistoryEasyData = createSelector(
    selectDrawHistoryState,
    (state: DrawHistoryState) => {
        if (state.drawHistory) {
            return state.drawHistory
                .filter((draw: OldRafflesReponse) => draw.lotteryId === 2)
                .map((draw: OldRafflesReponse) => ({ ...draw }));
        }
        return [];
    }
);
