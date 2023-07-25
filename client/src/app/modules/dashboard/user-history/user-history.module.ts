import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserHistoryRoutingModule } from './user-history-routing.module';
import { UserHistoryComponent } from './user-history.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { CoreModule } from 'src/app/core/core.module';
import { HistoryComponent } from './components/history/history.component';
import { StoreModule } from '@ngrx/store';
import { userHistoryReducer } from './features/history.reducer';


@NgModule({
  declarations: [
    UserHistoryComponent,
    HistoryComponent
  ],
  imports: [
    CommonModule,
    UserHistoryRoutingModule,
    SharedModule,
    CoreModule,
    StoreModule.forFeature('userHistory', userHistoryReducer),
  ]
})
export class UserHistoryModule {
  constructor() {
  }

  static forRoot(): ModuleWithProviders<UserHistoryModule> {
    return {
      ngModule: UserHistoryModule,
    };
  }
}
