import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserPackagesComponent } from './user-packages.component';
import {AuthGuard} from "../../../shared/guards/auth.guard";
import {UserPackageHomeComponent} from "./user-package-home/user-package-home.component";
import {UserPackagesResolver} from "./resolvers/user-packages.resolver";

const routes: Routes = [
  { path: '', component: UserPackagesComponent, canActivate: [AuthGuard],
    children: [
      {
        path: 'packages', component: UserPackageHomeComponent, resolve: {userPackage: UserPackagesResolver}
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserPackagesRoutingModule { }
