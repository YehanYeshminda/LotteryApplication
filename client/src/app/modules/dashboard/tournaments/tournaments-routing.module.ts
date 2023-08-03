import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TournamentsComponent } from './tournaments.component';
import {HomeComponent} from "./components/home/home.component";

const routes: Routes = [
    { path: '', component: TournamentsComponent, children: [
            {
                path: 'home', component: HomeComponent
            }
        ] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TournamentsRoutingModule { }
