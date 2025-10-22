import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginComponent } from './login/components/login.component';
import {AuthService} from "../../core/auth/auth.service";
import {CallbackComponent} from "./callback/callback.component";

@NgModule({
  declarations: [
    LoginComponent,
    CallbackComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    AuthRoutingModule,
  ],
  providers: [
    AuthService
  ]
})
export class AuthModule {}
