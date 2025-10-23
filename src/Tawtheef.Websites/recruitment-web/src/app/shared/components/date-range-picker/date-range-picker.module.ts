import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DateRangePickerComponent } from './date-range-picker.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  BsDatepickerModule,

} from 'ngx-bootstrap/datepicker';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BsDatepickerModule,
  ]
})
export class DateRangePickerModule { }
