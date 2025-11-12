import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { SharedModule } from '../../shared/shared.module';
import {TranslatePipe} from "@ngx-translate/core";
import {Login} from './login/login';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';

@NgModule({
  declarations: [
    Login
  ],
  imports: [
    CommonModule,
    SharedModule,
    AuthRoutingModule,
    TranslatePipe,
    I18nNamespaceDirective
  ]
})
export class AuthModule {}
