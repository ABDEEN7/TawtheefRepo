import { Injectable, inject } from "@angular/core";
import { Router } from "@angular/router";
import { routes } from '../../routes/routes';
import { PermissionService } from '../auth/permission.service';
import { Permissions } from '../constants/permissions';
import { SystemRoles } from '../constants/systemRoles';

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
    if (!role) {
      this.router.navigate([routes.accessDenied], { replaceUrl: true });
      return;
    }

    // 1. Dashboard (High Priority for Management roles)
    if (this.permissionService.hasPermission(Permissions.Dashboard.View)) {
      this.router.navigate([routes.dashboard(role)], { replaceUrl: true });
      return;
    }

    // 2. Jobs Management (Alternative for recruiters/staff)
    if (this.permissionService.hasPermission(Permissions.Jobs.View)) {
      this.router.navigate([routes.employee.JobList], { replaceUrl: true });
      return;
    }

    // 3. Office Users (Alternative for admins)
    if (this.permissionService.hasPermission(Permissions.OfficeUsers.View)) {
      this.router.navigate([routes.employee.officeUsersManagement], { replaceUrl: true });
      return;
    }

    // 4. Default Fallback for users with NO permissions yet (New Users)
    this.router.navigate([routes.auth.pendingApproval], { replaceUrl: true });
  }

  private getReturnUrl(): string | null {
    const tree = this.router.parseUrl(this.router.url);
    return tree.queryParams['returnUrl'] || null;
  }
}
