import { createReducer, on } from "@ngrx/store";
import { SingleUserInfo } from "../models/single-user";
import { UserInfoActions } from "./user-info.types";

export interface SingleUserInfoState {
    singleUserInfo: SingleUserInfo | undefined;
    isLoaded: boolean;
}

export const initialSingleUserInfoState: SingleUserInfoState = {
    singleUserInfo: undefined,
    isLoaded: false,
};

export const singleUserInfoReducer = createReducer(
    initialSingleUserInfoState,
    on(UserInfoActions.SaveSingleUserInfo, (state: any, action: any) => {
        return {
            ...state,
            singleUserInfo: action.singleUserInfo,
            isLoaded: true
        };
    }),
    on(UserInfoActions.RestoreSingleUserInfoInitialState, (state: any, action: any) => {
        return {
            ...state,
            singleUserInfo: undefined,
            isLoaded: false
        }
    }),
);