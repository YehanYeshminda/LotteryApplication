import { createAction, props } from "@ngrx/store";
import { SingleUserInfo, UpdateSingleUserInfo } from "../models/single-user";

export const SaveSingleUserInfo = createAction(
    '[EDIT USER Page] Single User Save All',
    props<{ singleUserInfo: SingleUserInfo }>()
);

export const RestoreSingleUserInfoInitialState = createAction(
    '[EDIT USER Page] SINGLE USER INFO Clear All'
);

export const UpdateSingleUserInfoAction = createAction(
    '[EDIT USER Page] SINGLE USER UPDATE',
    props<{ updateUserInfo: UpdateSingleUserInfo }>
);