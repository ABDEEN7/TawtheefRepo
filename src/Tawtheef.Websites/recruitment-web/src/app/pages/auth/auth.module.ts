import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { SharedModule } from '../../shared/shared.module';
import {TranslatePipe} from "@ngx-translate/core";
import {Login} from './login/login';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import { QatarResidentOtpDialogComponent } from './login/components/qatar-resident-otp-dialog/qatar-resident-otp-dialog.component';
import { DynamicDialogModule, DialogService } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

@NgModule({
  declarations: [
    Login,
    QatarResidentOtpDialogComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    AuthRoutingModule,
    TranslatePipe,
    I18nNamespaceDirective,
    DynamicDialogModule,
    InputTextModule,
    ButtonModule
  ],
  providers: [DialogService]
})
export class AuthModule {}
