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
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  errorMessage: string | null = null;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private external: ExternalLoginService,
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      remember: [false]
    });
  }

  loginWithProvider(provider: 'google' | 'microsoft' | 'azure' | 'sso') {
    switch (provider) {
      case 'azure':
        this.external.signInWithAzure();
        break;
      case 'google':
        this.external.signInWithGoogle();
        break;
    }
  }
  get f() { return this.loginForm.controls; }

  onForgotPassword() {
    // navigate to forgot password page
    this.router.navigate(['/forgot-password']);
  }
}
