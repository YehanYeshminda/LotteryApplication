import { createReducer, on } from "@ngrx/store";
import { Company } from "../models/lotto";
import { CompanyActions } from "./company.types";

export interface CompanyState {
    company: Company[] | undefined;
    isLoaded: boolean;
}

export const initialCompanyState: CompanyState = {
    company: undefined,
    isLoaded: false,
};

export const companyReducer = createReducer(
    initialCompanyState,
    on(CompanyActions.SaveCompany, (state: any, action: any) => {
        return {
            ...state,
            company: action.company,
            isLoaded: true,
        };
    }),
    on(CompanyActions.RestoreInitialState, (state: any, action: any) => {
        return {
            ...state,
            company: [],
            isLoaded: false
        }
    }),
);