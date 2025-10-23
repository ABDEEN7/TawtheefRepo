import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ErrorRoutingModule } from './error-routing.module';
import {TranslatePipe} from "@ngx-translate/core";


@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    ErrorRoutingModule,
    TranslatePipe,
  ]
})
export class ErrorModule { }
