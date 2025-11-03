import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {AuthService} from "../../../../core/auth/auth.service";
import {NgIf} from "@angular/common";
import {TranslatePipe} from "@ngx-translate/core";
import {ExternalLoginService} from '../../external-login';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    NgIf,
    ReactiveFormsModule,
    TranslatePipe,
  ],
  standalone: true
})
export class LoginComponent {
  loading = false;
  errorMessage: string | null = null;

  constructor(private external: ExternalLoginService,) {}

  loginWithProvider() {
    this.external.signInWithAzure().then(r => {});
  }
}
