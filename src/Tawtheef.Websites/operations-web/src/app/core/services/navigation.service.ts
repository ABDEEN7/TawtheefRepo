import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {routes} from '../../routes/routes';

@Injectable({providedIn: 'root'})
export class NavigationService {

  constructor(
    private router: Router
  ) {
  }

  navigateAfterLogin(mainUserRole: string): void {
    const returnUrl = this.getReturnUrl();
    if (returnUrl && this.isSafeReturnUrl(returnUrl)) {
      this.router.navigateByUrl(returnUrl).catch(() => this.redirectBasedOnRole(mainUserRole));
    } else {
      this.redirectBasedOnRole(mainUserRole);
    }
  }

  safeNavigateAfterLogin(mainUserRole: string) {
    try {
      this.navigateAfterLogin(mainUserRole);
    } catch {
      this.redirectBasedOnRole(mainUserRole);
    }
  }

  private isSafeReturnUrl(url: string): boolean {
    return !/^https?:\/\//i.test(url);
  }

  redirectBasedOnRole(mainUserRole: string): void {
    this.router.navigate([routes.dashboard(mainUserRole)], { replaceUrl: true });
  }
  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }
}
