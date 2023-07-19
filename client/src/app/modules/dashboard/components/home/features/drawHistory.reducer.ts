import { createReducer, on } from "@ngrx/store";
import { DrawHistoryActions } from "./drawHistory.types";
import { OldRafflesReponse } from "../models/home";

export interface DrawHistoryState {
    drawHistory: OldRafflesReponse[] | undefined;
    isLoaded: boolean;
}

export const initialDrawHistoryState: DrawHistoryState = {
    drawHistory: undefined,
    isLoaded: false,
};

export const drawHistoryReducer = createReducer(
    initialDrawHistoryState,
    on(DrawHistoryActions.SaveDrawHistory, (state: any, action: any) => {
        return {
            ...state,
            drawHistory: action.drawHistory,
            isLoaded: true,
        };
    })
);
