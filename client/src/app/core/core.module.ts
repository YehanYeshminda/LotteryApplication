import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './navbar/navbar.component';
import { RouterLink, RouterLinkActive } from "@angular/router";
import { FooterComponent } from './footer/footer.component';


@NgModule({
  declarations: [
    NavbarComponent,
    FooterComponent
  ],
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
  ],
    exports: [
        NavbarComponent,
        FooterComponent
    ],
})
export class CoreModule { }
