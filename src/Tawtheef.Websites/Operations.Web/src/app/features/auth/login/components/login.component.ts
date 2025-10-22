import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {AuthService} from "../../../../core/auth/auth.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
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

    const payload = {
      username: this.f.email.value,
      password: this.f.password.value
    };

    this.loading = true;
    this.auth.loginWithPassword(payload).subscribe({
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
    this.auth.startExternalLogin(provider, false);
  }

  onForgotPassword() {
    // navigate to forgot password page
    this.router.navigate(['/forgot-password']);
  }
}
