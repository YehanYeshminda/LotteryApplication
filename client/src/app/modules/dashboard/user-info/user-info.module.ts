import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserInfoRoutingModule } from './user-info-routing.module';
import { UserInfoComponent } from './user-info.component';
import { CoreModule } from 'src/app/core/core.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { EditUserComponent } from './components/edit-user/edit-user.component';
import { singleUserInfoReducer } from './features/user-info.reducer';
import { StoreModule } from '@ngrx/store';
import { SingleUserHttpService } from './services/single-user-http.service';
import { SingleUserInfoResolver } from './resolver/single-user-info.resolver';
import { PipesModule } from 'src/app/shared/pipes/pipes.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    UserInfoComponent,
    EditUserComponent
  ],
  providers: [
    SingleUserHttpService,
    SingleUserInfoResolver,
  ],
  imports: [
    CommonModule,
    UserInfoRoutingModule,
    CoreModule,
    SharedModule,
    UserInfoRoutingModule,
    StoreModule.forFeature('singleUserInfo', singleUserInfoReducer),
    PipesModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class UserInfoModule { }
