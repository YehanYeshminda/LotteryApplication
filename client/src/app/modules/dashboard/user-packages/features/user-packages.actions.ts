import {createAction, props} from "@ngrx/store";
import {UserPackage} from "../models/user-package";

export const SaveUserPackageHistory = createAction(
  '[UserHistory Page] User History Save All',
  props<{ userPackages: UserPackage[] }>()
);
