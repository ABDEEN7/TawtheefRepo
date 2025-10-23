import {Injectable} from "@angular/core";
import {AuthCoreService} from "./auth-core.service";
import {AuthStateService} from "./auth-state.service";
import {UserService} from "./user.service";
import {Observable, of} from "rxjs";
import {UserInfoModel} from "../../shared/models/user-info.model";
import {LoginResponse} from "../../features/auth/login/models/login.model";

@Injectable({providedIn: 'root'})
export class AuthService {
  constructor(
    public core: AuthCoreService,
    public state: AuthStateService,
    public user: UserService
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

  login(email: string, password: string): Observable<boolean> {
    return this.core.login(email, password);
  }

  externalLogin(data: LoginResponse) {
    return this.core.externalLogin(data);
  }

  register(fullName: string, email: string, gender: string, password: string, type: string): Observable<boolean> {
    return this.core.register(fullName, email, gender, password, type);
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
