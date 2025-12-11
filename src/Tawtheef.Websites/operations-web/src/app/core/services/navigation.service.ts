import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {routes} from '../../routes/routes';

@Injectable({providedIn: 'root'})
export class NavigationService {

  constructor(
    private router: Router
  ) {
  }

  navigateAfterLogin(userType: string): void {
    const returnUrl = this.getReturnUrl();
    if (returnUrl && this.isSafeReturnUrl(returnUrl)) {
      this.router.navigateByUrl(returnUrl).catch(() => this.redirectBasedOnRole(userType));
    } else {
      this.redirectBasedOnRole(userType);
    }
  }

  safeNavigateAfterLogin(userType: string) {
    try {
      this.navigateAfterLogin(userType);
    } catch {
      this.redirectBasedOnRole(userType);
    }
  }

  private isSafeReturnUrl(url: string): boolean {
    return !/^https?:\/\//i.test(url);
  }

  redirectBasedOnRole(userType: string): void {
    this.router.navigate([routes.dashboard(userType)], { replaceUrl: true });
  }
  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }
}
