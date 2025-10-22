import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {AuthService} from "../../../core/auth/auth.service";

@Component({ template: '<p>Logging in...</p>' })
export class CallbackComponent implements OnInit {
  constructor(private route: ActivatedRoute, private auth: AuthService, private router: Router) {}

  ngOnInit() {
    const code = this.route.snapshot.queryParamMap.get('code');
    const state = this.route.snapshot.queryParamMap.get('state');
    const provider = this.route.snapshot.queryParamMap.get('provider') || 'google'; // or derive from state
    if (!code) {
      this.router.navigateByUrl('/login');
      return;
    }

    this.auth.finishLoginByCode(provider, code, state).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: () => this.router.navigateByUrl('/login?error=1')
    });
  }
}
