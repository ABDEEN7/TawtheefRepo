import {Injectable} from "@angular/core";
import {AuthCoreService} from "./auth-core.service";
import {AuthStateService} from "./auth-state.service";
import {UserService} from "./user.service";
import {Observable, of} from "rxjs";
import {UserInfoModel} from "../../shared/models/user-info.model";
import {AuthResponse} from "../../features/auth/login/models/auth-response.model";

declare var google: any;
@Injectable({providedIn: 'root'})
export class AuthService {
  constructor(
    protected core: AuthCoreService,
    protected state: AuthStateService,
    protected user: UserService,
  ) {
    this.state.checkAuthState(false);
  }

  // Proxy methods for convenience
  get isAuthenticated$(): Observable<boolean> {
    return this.state.isAuthenticated$;
  }

  get token(): string | null {
    return this.core.getToken;
  }

  get currentUser$(): Observable<UserInfoModel | null> {
    return this.user.currentUser$;
  }

  get getCurrentUserRole() {
    return () => this.user.getCurrentUser()?.userType || '';
  }

  externalLogin(data: AuthResponse) {
    return this.core.externalLogin(data);
  }

  logout(): void {
    this.state.logout();
  }

  refreshToken(): Observable<string | null> {
    return this.core.refreshToken();
  }

  getCurrentUser(): UserInfoModel | null {
    return this.user.getCurrentUser();
  }

  isPasswordlessUser(): boolean {
    return this.user.isPasswordlessUser();
  }

  isLoggedIn(): boolean {
    return this.state.checkAuthState(false);
  }

  deleteAccount(): void {
    this.state.logout(false);
  }
}
