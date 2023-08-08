import { createFeatureSelector, createSelector } from "@ngrx/store";
import { CompanyState } from "./company.reducer";

export const selectCompanyState = createFeatureSelector<CompanyState>('company');

export const selectCompanyLoaded = createSelector(
    selectCompanyState,
    (state: CompanyState) => state.isLoaded
);

export const selectCompanyData = createSelector(
    selectCompanyState,
    (state: CompanyState) => state.company
);