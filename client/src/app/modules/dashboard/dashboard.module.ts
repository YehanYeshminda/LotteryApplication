import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
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


const entityMetaData: EntityMetadataMap = {
  Cart: {},
}

@NgModule({
  declarations: [
    DashboardComponent,
    EasyDrawComponent,
    MegaDrawComponent,
    CartComponent,
    PaymentComponent,
    SpashScreenComponent,
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
    ReactiveFormsModule
  ],
  providers: [
    CartEntityService,
    CartHttpService,
    CartResolver,
    CartDataService
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})

export class DashboardModule {
  constructor(private eds: EntityDefinitionService, private entityDataService: EntityDataService, private cartDataService: CartDataService) {
    eds.registerMetadataMap(entityMetaData);
    entityDataService.registerService("Cart", cartDataService);
  }
}


