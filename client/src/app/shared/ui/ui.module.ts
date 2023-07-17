import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputModule } from "./input/input.module";
import { CarouselModule } from './carousel/carousel.module';

@NgModule({
  declarations: [],
  imports: [CommonModule, InputModule, CarouselModule],
  exports: [InputModule, CarouselModule],
})
export class UiModule { }
