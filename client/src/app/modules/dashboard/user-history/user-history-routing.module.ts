import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserHistoryComponent } from './user-history.component';
import { AuthGuard } from 'src/app/shared/guards/auth.guard';
import { HistoryComponent } from './components/history/history.component';
import { UserHistoryResolver } from './resolvers/user-history.resolver';

const routes: Routes = [
  {
    path: '', component: UserHistoryComponent, canActivate: [AuthGuard],
    children: [
      {
        path: 'user-history',
        component: HistoryComponent,
        resolve: { userHistoryData: UserHistoryResolver }
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserHistoryRoutingModule { }
