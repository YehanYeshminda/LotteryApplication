import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TournamentsRoutingModule } from './tournaments-routing.module';
import { TournamentsComponent } from './tournaments.component';
import { HomeComponent } from './components/home/home.component';
import {CoreModule} from "../../../core/core.module";
import {PipesModule} from "../../../shared/pipes/pipes.module";


@NgModule({
  declarations: [
    TournamentsComponent,
    HomeComponent
  ],
    imports: [
        CommonModule,
        TournamentsRoutingModule,
        CoreModule,
        PipesModule
    ]
})
export class TournamentsModule { }
