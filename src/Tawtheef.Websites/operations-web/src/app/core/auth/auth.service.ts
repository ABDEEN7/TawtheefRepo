import { Injectable, inject } from "@angular/core";
import { AuthCoreService } from "./auth-core.service";
import { AuthStateService } from "./auth-state.service";
import { UserService } from "./user.service";
import { PermissionService } from "./permission.service";
import { Observable } from "rxjs";
import { UserInfoModel } from "../../shared/models/user-info.model";
import { AuthResponse } from '../models/auth-response.model';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  protected core = inject(AuthCoreService);
  protected state = inject(AuthStateService);
  protected user = inject(UserService);
  protected permissionService = inject(PermissionService);

  constructor() {
    this.state.checkAuthState();
  }

  // Proxy methods for convenience
  get isAuthenticated$(): Observable<boolean> {
    return this.state.isAuthenticated$;
  }
  get isAuthenticated(): boolean {
    return this.state.isAuthenticated();
  }

  get token(): string | null {
    return this.core.getToken;
  }

  get currentUser$(): Observable<UserInfoModel | null> {
    return this.user.currentUser$;
  }

  externalLogin(data: AuthResponse) {
    return this.core.externalLogin(data);
  }

  logout(callServer: boolean = true): void {
    this.state.logout(callServer);
  }

  refreshToken(): Observable<string | null> {
    return this.state.refreshAccessToken$().pipe(
      map((ok) => (ok ? this.core.getToken : null)),
    );
  }

  getCurrentUser(): UserInfoModel | null {
    return this.user.getCurrentUser();
  }

  isLoggedIn(): boolean {
    return this.state.checkAuthState();
  }

  /**
   * Check if user has at least one / all of the required permissions.
   *
   * @param permission single permission or array
   * @param requireAll if true, all permissions must be present (AND); default: false (OR)
   */
  hasPermission(permission: string | string[], requireAll: boolean = false): boolean {
    return this.permissionService.hasPermission(permission, requireAll);
  }
}
