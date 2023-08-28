import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PolicysComponent } from './policys.component';
import { AuthGuard } from '@shared/guards/auth.guard';
import { RulesComponent } from './components/rules/rules.component';
import { PolicyComponent } from './components/policy/policy.component';

const routes: Routes = [
  {
    path: '',
    component: PolicysComponent, // Initialize PolicysComponent
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        redirectTo: 'rules',
        pathMatch: 'full'
      },
      {
        path: 'rules',
        component: RulesComponent,
      },
      {
        path: 'terms',
        component: PolicyComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PolicysRoutingModule { }
