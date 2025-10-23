import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {AuthService} from "../../../../core/auth/auth.service";
import {NgIf} from "@angular/common";
import {TranslatePipe} from "@ngx-translate/core";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    NgIf,
    ReactiveFormsModule,
    RouterLink,
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
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      remember: [false]
    });
  }

  get f() { return this.loginForm.controls; }

  onSubmit() {
    this.errorMessage = null;
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.auth.login(this.f['email'].value,this.f['password'].value).subscribe({
      next: () => {
        // Navigate to home or intended URL
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        console.error(err);
        // Customize userModel-friendly messages based on error.status / body
        this.errorMessage = err?.error?.message || 'فشل تسجيل الدخول. تحقق من البيانات وحاول مرة أخرى.';
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  // Social / SSO login
  loginWithProvider(provider: 'google' | 'microsoft' | 'azure' | 'sso') {
    // Option: open popup or redirect - here we redirect
    //this.auth.externalLogin();
  }

  onForgotPassword() {
    // navigate to forgot password page
    this.router.navigate(['/forgot-password']);
  }
}
