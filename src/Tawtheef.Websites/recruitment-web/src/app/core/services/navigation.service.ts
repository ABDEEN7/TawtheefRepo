import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {routes} from '../../routes/routes';

@Injectable({providedIn: 'root'})
export class NavigationService {

  constructor(
    private router: Router
  ) {
  }

  navigateAfterLogin(): void {
    const returnUrl = this.getReturnUrl();
    if (returnUrl && this.isSafeReturnUrl(returnUrl)) {
      this.router.navigateByUrl(returnUrl).catch(() => this.redirectBasedOnRole());
    } else {
      this.redirectBasedOnRole();
    }
  }

  safeNavigateAfterLogin() {
    try {
      this.navigateAfterLogin();
    } catch {
      this.redirectBasedOnRole();
    }
  }

  private isSafeReturnUrl(url: string): boolean {
    return !/^https?:\/\//i.test(url);
  }

  redirectBasedOnRole(): void {
    this.router.navigate([routes.user.dashboard], { replaceUrl: true });
  }
  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }
}
