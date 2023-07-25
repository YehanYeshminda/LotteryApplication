import { CUSTOM_ELEMENTS_SCHEMA, ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { CoreModule } from 'src/app/core/core.module';
import { EasyDrawComponent } from './components/easy-draw/easy-draw.component';
import { MegaDrawComponent } from './components/mega-draw/mega-draw.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { CartComponent } from './components/cart/cart.component';
import { PaymentComponent } from './components/payment/payment.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { SpashScreenComponent } from './components/spash-screen/spash-screen.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ProgressbarModule } from 'ngx-bootstrap/progressbar';
import { NgxStripeModule } from 'ngx-stripe';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EntityDataService, EntityDefinitionService, EntityMetadataMap } from '@ngrx/data';
import { CartEntityService } from './components/cart/services/cart-entity.service';
import { CartHttpService } from './components/cart/services/cart-http.service';
import { CartResolver } from './components/cart/resolver/cart.resolver';
import { CartDataService } from './components/cart/services/cart-data.service';
import { HomeComponent } from './components/home/home.component';
import { HomeDataService } from './components/home/services/home-data.service';
import { SequenceNoResolver } from './components/home/resolvers/sequence-no.resolver';
import { HomeEntityService } from './components/home/services/home-entity.service';
import { MegaDrawResolver } from './components/mega-draw/resolvers/mega-draw.resolver';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { StoreModule } from '@ngrx/store';
import { drawHistoryReducer } from './components/home/features/drawHistory.reducer';
import { DrawHistoryResolver } from './components/home/resolvers/draw-history.resolver';
import { MatStepperModule } from '@angular/material/stepper';
import { MatButtonModule } from '@angular/material/button';

const entityMetaData: EntityMetadataMap = {
  Cart: {
    entityDispatcherOptions: {
      optimisticDelete: true,
      optimisticUpdate: true
    }
  },
  Home: {},
}

@NgModule({
  declarations: [
    DashboardComponent,
    EasyDrawComponent,
    MegaDrawComponent,
    CartComponent,
    PaymentComponent,
    SpashScreenComponent,
    HomeComponent,
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    CoreModule,
    SharedModule,
    NgxSpinnerModule,
    ModalModule.forRoot(),
    ProgressbarModule.forRoot(),
    NgxStripeModule.forRoot('pk_test_51NSgQVCGctxV4GttmBY8TDMVypTxkWkWqc8w8AfeEFXDRYv93CoqNnSLOuClW6rCevuODvzwLXGEoNj2PYRBTVIU00qutXHWvA'),
    FormsModule,
    ReactiveFormsModule,
    CarouselModule.forRoot(),
    StoreModule.forFeature('drawHistory', drawHistoryReducer),
    MatStepperModule,
    MatButtonModule
  ],
  providers: [
    CartEntityService,
    CartHttpService,
    CartResolver,
    CartDataService,
    HomeDataService,
    HomeEntityService,
    SequenceNoResolver,
    MegaDrawResolver,
    DrawHistoryResolver
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})

export class DashboardModule {
  constructor(private eds: EntityDefinitionService, private entityDataService: EntityDataService, private cartDataService: CartDataService, private HomeDataService: HomeDataService) {
    eds.registerMetadataMap(entityMetaData);
    entityDataService.registerService("Cart", cartDataService);
    entityDataService.registerService("Home", HomeDataService);
  }

  static forRoot(): ModuleWithProviders<DashboardModule> {
    return {
      ngModule: DashboardModule,
    };
  }
}


