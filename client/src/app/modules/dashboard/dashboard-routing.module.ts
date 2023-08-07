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
import { SearchHistoryComponent } from './components/search-history/search-history.component';
import { UserHomeComponent } from './components/user-home/user-home.component';
import {LottoComponent} from "./components/lotto/lotto.component";

const routes: Routes = [
  {
    path: '', component: DashboardComponent, resolve: { CartItems: CartResolver }, canActivate: [AuthGuard], children: [
      { path: 'home', component: HomeComponent, resolve: { CartItems: CartResolver, HomeItems: SequenceNoResolver, DrawHistory: DrawHistoryResolver } },
      { path: 'user-home', component: UserHomeComponent },
      { path: 'easy-draw', component: EasyDrawComponent, resolve: { CartItems: CartResolver, DrawHistory: DrawHistoryResolver } },
      { path: 'mega-draw', component: MegaDrawComponent, resolve: { CartItems: CartResolver, drawNumbers: MegaDrawResolver, DrawHistory: DrawHistoryResolver } },
      { path: 'lotto', component: LottoComponent, resolve: { CartItems: CartResolver } },
      { path: 'cart', component: CartComponent, resolve: { CartItems: CartResolver } },
      { path: 'checkout', component: CheckoutComponent, resolve: { CartItems: CartResolver } },
      { path: 'search-history/:id', component: SearchHistoryComponent },
    ]
  },
  { path: 'history', loadChildren: () => import('./user-history/user-history.module').then(m => m.UserHistoryModule), canActivate: [AuthGuard] },
  { path: 'info', loadChildren: () => import('./user-info/user-info.module').then(m => m.UserInfoModule), canActivate: [AuthGuard] },
  { path: 'user-package', loadChildren: () => import('./user-packages/user-packages.module').then(m => m.UserPackagesModule), canActivate: [AuthGuard] },
  { path: 'tournaments', loadChildren: () => import('./tournaments/tournaments.module').then(m => m.TournamentsModule), canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
