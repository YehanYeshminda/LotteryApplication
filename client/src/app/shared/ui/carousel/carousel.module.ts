import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarouselModule as Carousel } from 'ngx-bootstrap/carousel';
import { CarouselComponent } from './carousel.component';

@NgModule({
  declarations: [
    CarouselComponent
  ],
  imports: [
    CommonModule,
    Carousel.forRoot()
  ],
  exports: [
    CarouselComponent
  ],
})
export class CarouselModule { }
