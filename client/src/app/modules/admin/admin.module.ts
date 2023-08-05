import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { HomeComponent } from './components/home/home.component';
import {CoreModule} from "../../core/core.module";


@NgModule({
  declarations: [
    AdminComponent,
    HomeComponent
  ],
    imports: [
        CommonModule,
        AdminRoutingModule,
        CoreModule
    ]
})
export class AdminModule { }
