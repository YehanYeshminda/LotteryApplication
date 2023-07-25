import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { EasyDrawComponent } from './components/easy-draw/easy-draw.component';
import { MegaDrawComponent } from './components/mega-draw/mega-draw.component';
import { CartComponent } from "./components/cart/cart.component";
import { AuthGuard } from "../../shared/guards/auth.guard";
import { CartResolver } from './components/cart/resolver/cart.resolver';
import { HomeComponent } from './components/home/home.component';
import { SequenceNoResolver } from './components/home/resolvers/sequence-no.resolver';
import { MegaDrawResolver } from './components/mega-draw/resolvers/mega-draw.resolver';
import { DrawHistoryResolver } from './components/home/resolvers/draw-history.resolver';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { UserHistoryComponent } from './components/user-history/user-history.component';

const routes: Routes = [
  {
    path: '', component: DashboardComponent, resolve: { CartItems: CartResolver }, canActivate: [AuthGuard], children: [
      { path: 'home', component: HomeComponent, resolve: { CartItems: CartResolver, HomeItems: SequenceNoResolver, DrawHistory: DrawHistoryResolver } },
      { path: 'easy-draw', component: EasyDrawComponent, resolve: { CartItems: CartResolver, DrawHistory: DrawHistoryResolver } },
      { path: 'mega-draw', component: MegaDrawComponent, resolve: { CartItems: CartResolver, drawNumbers: MegaDrawResolver, DrawHistory: DrawHistoryResolver } },
      { path: 'cart', component: CartComponent, resolve: { CartItems: CartResolver } },
      { path: 'checkout', component: CheckoutComponent, resolve: { CartItems: CartResolver } },
    ]
  },
  { path: 'history', loadChildren: () => import('./user-history/user-history.module').then(m => m.UserHistoryModule), canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
