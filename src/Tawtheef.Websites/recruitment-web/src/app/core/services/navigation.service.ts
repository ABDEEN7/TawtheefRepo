import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenService } from '../auth/token.service';
import { routes } from '../../routes/routes';

@Injectable({ providedIn: 'root' })
export class NavigationService {

  constructor(
    private router: Router,
    private tokens: TokenService
  ) {
  }

  navigateAfterLogin(requiresProfileCompletion?: boolean): void {
    const returnUrl = this.getReturnUrl();
    if (returnUrl && this.isSafeReturnUrl(returnUrl)) {
      this.router.navigateByUrl(returnUrl).catch(() => this.redirectBasedOnRole(requiresProfileCompletion));
    } else {
      this.redirectBasedOnRole(requiresProfileCompletion);
    }
  }

  safeNavigateAfterLogin(requiresProfileCompletion?: boolean) {
    try {
      this.navigateAfterLogin(requiresProfileCompletion);
    } catch {
      this.redirectBasedOnRole(requiresProfileCompletion);
    }
  }

  private isSafeReturnUrl(url: string): boolean {
    return !/^https?:\/\//i.test(url);
  }

  redirectBasedOnRole(requiresProfileCompletion?: boolean): void {
    const isProfileCompleted = requiresProfileCompletion !== undefined
      ? !requiresProfileCompletion
      : this.tokens.isProfileComplete();

    const targetRoute = isProfileCompleted
      ? routes.user.dashboard
      : routes.user.profileWizard;

    this.router.navigate([targetRoute], { replaceUrl: true });
  }
  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }
}
