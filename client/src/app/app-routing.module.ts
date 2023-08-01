import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '', loadChildren: () => import('./modules/dashboard/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'dashboard', loadChildren: () => import('./modules/dashboard/dashboard.module').then(m => m.DashboardModule), canActivate: [AuthGuard]
  },
  { path: 'admin', loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule) },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
