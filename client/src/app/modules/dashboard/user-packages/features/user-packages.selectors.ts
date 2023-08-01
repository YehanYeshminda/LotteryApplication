import {createFeatureSelector, createSelector} from "@ngrx/store";
import {UserPackagesState} from "./user-packages.reducer";

export const selectUserPackageState = createFeatureSelector<UserPackagesState>('userPackages');

export const selectUserPackageDataLoaded = createSelector(
  selectUserPackageState,
  (state: UserPackagesState) => state.isLoaded
);

export const selectUserPackageData = createSelector(
  selectUserPackageState,
  (state: UserPackagesState) => state.userPackages
);
