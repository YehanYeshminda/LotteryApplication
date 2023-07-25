import { createReducer, on } from "@ngrx/store";
import { UserHistoryResponse } from "../models/userhistory";
import { UserHistoryActions } from "./history.types";

export interface UserHistoryState {
    userHistory: UserHistoryResponse[] | undefined;
    isLoaded: boolean;
}

export const initialDrawHistoryState: UserHistoryState = {
    userHistory: undefined,
    isLoaded: false,
};

export const userHistoryReducer = createReducer(
    initialDrawHistoryState,
    on(UserHistoryActions.SaveUserHistory, (state: any, action: any) => {
        return {
            ...state,
            userHistory: action.userHistory,
            isLoaded: true,
        };
    })
);