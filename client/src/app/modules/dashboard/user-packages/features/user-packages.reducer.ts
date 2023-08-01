import {UserPackage} from "../models/user-package";
import {createReducer, on} from "@ngrx/store";
import {UserPackagesActions} from "./user-packages.types";

export interface UserPackagesState {
  userPackages: UserPackage[] | undefined;
  isLoaded: boolean;
}

export const initialUserPackageState: UserPackagesState = {
  userPackages: undefined,
  isLoaded: false,
};

export const userPackageReducer = createReducer(
  initialUserPackageState,
  on(UserPackagesActions.SaveUserPackageHistory, (state: any, action: any) => {
    return {
      ...state,
      userPackages: action.userPackages,
      isLoaded: true,
    };
  }),
);
