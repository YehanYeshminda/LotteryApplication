import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserInfoComponent } from './user-info.component';
import { AuthGuard } from 'src/app/shared/guards/auth.guard';
import { EditUserComponent } from './components/edit-user/edit-user.component';
import { SingleUserInfoResolver } from './resolver/single-user-info.resolver';

const routes: Routes = [{
  path: '', component: UserInfoComponent, canActivate: [AuthGuard], children: [{
    path: 'edit-user',
    component: EditUserComponent,
    resolve: {
      singleUserInfo: SingleUserInfoResolver
    }
  }]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserInfoRoutingModule { }
