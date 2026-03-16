import { Injectable, inject } from "@angular/core";
import { Router } from "@angular/router";
import { routes } from '../../routes/routes';
import { PermissionService } from '../auth/permission.service';
import { Permissions } from '../constants/permissions';
import { SystemRoles } from '../constants/systemRoles';
import { Sidebar } from "../../layouts/admin/sidebar/sidebar.models";

@Injectable({ providedIn: 'root' })
export class NavigationService {
  private router = inject(Router);
  private permissionService = inject(PermissionService);

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

  redirectBasedOnRole(role: string): void {
    const commands = this.getRoleBasedRoute(role);
    this.router.navigate(commands, { replaceUrl: true });
  }

  getRoleBasedRoute(role: string): string[] {
    if (!role) {
      return [routes.accessDenied];
    }

    const item = Sidebar.menuItems.find(item =>
      this.permissionService.hasPermission(item.permission)
    );

    if (item) {
      return [item.route];
    }

    // 4. Default Fallback for users with NO permissions yet (New Users)
    return [routes.auth.pendingApproval];
  }

  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }
}
