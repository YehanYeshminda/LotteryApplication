import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimezoneConverterPipe } from './timezonePipe/timezone-converter-pipe.pipe';



@NgModule({
  declarations: [
    TimezoneConverterPipe
  ],
  imports: [
    CommonModule
  ],
  exports: [
    TimezoneConverterPipe
  ]
})
export class PipesModule { }
