import { ModuleWithProviders, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { UiModule } from 'src/app/shared/ui/ui.module';
import { StoreModule } from '@ngrx/store';
import { authReducer } from './reducer/auth.reducer';
import { EffectsModule } from '@ngrx/effects';
import { AuthEffects } from './features/auth.effects';
import { ModalModule } from 'ngx-bootstrap/modal';

@NgModule({
  declarations: [
    AuthComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    SharedModule,
    UiModule,
    ReactiveFormsModule,
    FormsModule,
    ModalModule.forRoot(),
    StoreModule.forFeature('auth', authReducer),
    EffectsModule.forFeature([AuthEffects]),
  ]
})
export class AuthModule {
  static forRoot(): ModuleWithProviders<AuthModule> {
    return {
      ngModule: AuthModule,
    };
  }
}
