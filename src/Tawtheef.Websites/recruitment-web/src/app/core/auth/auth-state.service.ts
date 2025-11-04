import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, of, tap } from 'rxjs';
import { Router, UrlTree } from '@angular/router';
import { HttpService } from '../http/http.service';
import { EndpointsService } from '../http/endpoints.service';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { routes } from '../../routes/routes';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  readonly routes = routes;
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private router: Router,
    private http: HttpService,
    private endpoints: EndpointsService,
    private tokenService: TokenService,
    private userService: UserService
  ) {}

  /** one-shot setter for components/services */
  setAuthenticated(value: boolean): void {
    this.isAuthenticatedSubject.next(value);
  }

  /** quick sync check (no redirects). safe for guards. */
  isAuthenticated(): boolean {
    const token = this.tokenService.getToken();
    const data = localStorage.getItem('user_data');
    if (!token || this.tokenService.isTokenExpired(token) || !data) {
      this.isAuthenticatedSubject.next(false);
      return false;
    }
    // keep BehaviorSubject in sync
    if (!this.isAuthenticatedSubject.value) {
      try {
        const user = JSON.parse(data);
        this.userService.updateCurrentUser(user, token);
      } catch { /* ignore parse error here */ }
      this.isAuthenticatedSubject.next(true);
    }
    return true;
  }

  /**
   * guard helper: if not authed, return UrlTree to login with returnUrl
   * use inside canMatch/canActivateChild
   */
  ensureAuth(returnUrl?: string): true | UrlTree {
    if (this.isAuthenticated()) return true;
    return this.router.createUrlTree(
      [this.routes.auth.login], // e.g. '/auth/login'
      returnUrl ? { queryParams: { returnUrl } } : undefined
    );
  }

  /** full check that may redirect to logout (use in app init flows) */
  checkAuthState(allowRedirectLogout: boolean): boolean {
    const ok = this.isAuthenticated();
    if (!ok && allowRedirectLogout) this.logout(false); // local clear only
    return ok;
  }

  /** robust logout: clears locally even if API fails */
  logout(callServer: boolean = true): void {
    const doLocalClear = () => {
      this.tokenService.clearTokens();
      this.userService.clearCurrentUser();
      this.setAuthenticated(false);
      this.router.navigate([this.routes.home]); // e.g. '/'
    };

    if (!callServer) return doLocalClear();

    this.http.post(this.endpoints.auth.logout, {})
      .pipe(
        tap(() => doLocalClear()),
        catchError(() => { doLocalClear(); return of(null); })
      )
      .subscribe();
  }
}
