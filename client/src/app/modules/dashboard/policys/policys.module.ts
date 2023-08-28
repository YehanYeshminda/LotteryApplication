import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PolicysRoutingModule } from './policys-routing.module';
import { PolicysComponent } from './policys.component';
import { CoreModule } from 'src/app/core/core.module';
import { PolicyComponent } from './components/policy/policy.component';
import { RulesComponent } from './components/rules/rules.component';


@NgModule({
  declarations: [
    PolicysComponent,
    PolicyComponent,
    RulesComponent
  ],
  imports: [
    CommonModule,
    PolicysRoutingModule,
    CoreModule
  ]
})
export class PolicysModule { }
