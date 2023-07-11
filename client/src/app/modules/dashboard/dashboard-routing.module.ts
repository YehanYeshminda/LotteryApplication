import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { EasyDrawComponent } from './components/easy-draw/easy-draw.component';
import { MegaDrawComponent } from './components/mega-draw/mega-draw.component';
import {CartComponent} from "./components/cart/cart.component";
import {AuthGuard} from "../../shared/guards/auth.guard";

const routes: Routes = [
  { path: '', component: DashboardComponent, canActivate: [AuthGuard], children: [
      { path: 'easy-draw', component: EasyDrawComponent },
      { path: 'mega-draw', component: MegaDrawComponent },
      { path: 'cart', component: CartComponent }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
