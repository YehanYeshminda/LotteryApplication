import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserPackagesRoutingModule } from './user-packages-routing.module';
import { UserPackagesComponent } from './user-packages.component';
import { UserPackageHomeComponent } from './user-package-home/user-package-home.component';
import {CoreModule} from "../../../core/core.module";
import {StoreModule} from "@ngrx/store";
import {userPackageReducer} from "./features/user-packages.reducer";


@NgModule({
  declarations: [
    UserPackagesComponent,
    UserPackageHomeComponent
  ],
  imports: [
    CommonModule,
    UserPackagesRoutingModule,
    CoreModule,
    StoreModule.forFeature('userPackages', userPackageReducer),
  ]
})
export class UserPackagesModule { }
