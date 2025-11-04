import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { SharedModule } from '../../shared/shared.module';
import {TranslatePipe} from "@ngx-translate/core";

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    SharedModule,
    AuthRoutingModule,
    TranslatePipe
  ]
})
export class AuthModule {}
