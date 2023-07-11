import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { CoreModule } from 'src/app/core/core.module';
import { EasyDrawComponent } from './components/easy-draw/easy-draw.component';
import { MegaDrawComponent } from './components/mega-draw/mega-draw.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CartComponent } from './components/cart/cart.component';


@NgModule({
  declarations: [
    DashboardComponent,
    EasyDrawComponent,
    MegaDrawComponent,
    CartComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    CoreModule,
    ModalModule.forRoot(),
  ]
})

export class DashboardModule {}


