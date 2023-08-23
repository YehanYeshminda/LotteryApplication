import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserHistoryRoutingModule } from './user-history-routing.module';
import { UserHistoryComponent } from './user-history.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { CoreModule } from 'src/app/core/core.module';
import { HistoryComponent } from './components/history/history.component';
import { StoreModule } from '@ngrx/store';
import { userHistoryReducer } from './features/history.reducer';
import { UserHistoryResolver } from './resolvers/user-history.resolver';
import { HistorycardComponent } from './components/historycard/historycard.component';
import { MatTabsModule } from '@angular/material/tabs';
import { TimezoneConverterPipe } from 'src/app/shared/pipes/timezonePipe/timezone-converter-pipe.pipe';
import { PipesModule } from 'src/app/shared/pipes/pipes.module';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TransactionHistoryComponent } from './components/transaction-history/transaction-history.component';
import { TransactionCardComponent } from './components/transaction-history/components/transaction-card/transaction-card.component';
import { TransactionLosingCardComponent } from './components/transaction-history/components/transaction-losing-card/transaction-losing-card.component';
import { LottoHistoryComponent } from './components/lotto-history/lotto-history.component';
import { LottoHistoryCardComponent } from './components/lotto-history-card/lotto-history-card.component';

@NgModule({
  declarations: [
    UserHistoryComponent,
    HistoryComponent,
    HistorycardComponent,
    TransactionHistoryComponent,
    TransactionCardComponent,
    TransactionLosingCardComponent,
    LottoHistoryComponent,
    LottoHistoryCardComponent,
  ],
  imports: [
    CommonModule,
    UserHistoryRoutingModule,
    SharedModule,
    PipesModule,
    CoreModule,
    StoreModule.forFeature('userHistory', userHistoryReducer),
    MatTabsModule,
    InfiniteScrollModule,
    NgxSpinnerModule.forRoot({ type: 'ball-scale-multiple' })
  ],
  providers: [
    UserHistoryResolver,
    TimezoneConverterPipe
  ],
  exports: [
    HistorycardComponent,
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
