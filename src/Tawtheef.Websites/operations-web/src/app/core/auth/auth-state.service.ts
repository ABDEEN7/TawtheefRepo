import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, finalize, map, Observable, of, shareReplay, tap } from 'rxjs';
import { Router, UrlTree } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

import { HttpService } from '../http/http.service';
import { EndpointsService } from '../http/endpoints.service';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { routes } from '../../routes/routes';
import { HDR } from '../utils/headers.flags';

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  readonly routes = routes;

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  // De-dupe concurrent refresh calls
  private refreshInFlight$?: Observable<boolean> | null;

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

  /** quick sync check (no redirects). safe for UI rendering. */
  isAuthenticated(ignoreExpired = false): boolean {
    const token = this.tokenService.getToken();
    const data = localStorage.getItem('user_data');

    if (!token || !data) {
      this.isAuthenticatedSubject.next(false);
      return false;
    }

    if (!ignoreExpired && this.tokenService.isTokenExpired(token)) {
      this.isAuthenticatedSubject.next(false);
      return false;
    }

    // keep BehaviorSubject in sync
    if (!this.isAuthenticatedSubject.value) {
      try {
        const user = JSON.parse(data);
        this.userService.updateCurrentUser(user, token);
      } catch {}
      this.isAuthenticatedSubject.next(true);
    }

    return true;
  }

  /**
   * Guard SYNC helper (no HTTP). If you need refresh logic in guards, use ensureAuth$.
   */
  ensureAuth(returnUrl?: string): true | UrlTree {
    if (this.isAuthenticated(true)) return true; // ignoreExpired=true => just checks presence
    return this.router.createUrlTree(
      [this.routes.auth.login],
      returnUrl ? { queryParams: { returnUrl } } : undefined
    );
  }

  /**
   * Guard/Init ASYNC helper (with refresh logic).
   * - If access token valid => true
   * - If expired => try refresh (only if refresh token can be attempted)
   * - If refresh fails => UrlTree to login
   */
  ensureAuth$(returnUrl?: string): Observable<true | UrlTree> {
    // already valid
    if (this.isAuthenticated(false)) return of(true);

    const token = this.tokenService.getToken();
    const hasExpiredAccess = !!token && this.tokenService.isTokenExpired(token);

    // no token OR not an "expired token" case => login
    if (!hasExpiredAccess) {
      return of(
        this.router.createUrlTree(
          [this.routes.auth.login],
          returnUrl ? { queryParams: { returnUrl } } : undefined
        )
      );
    }

    // expired access => attempt refresh
    return this.refreshSession$().pipe(
      map((ok) => {
        if (ok) return true as const;

        // refresh failed => clear and go login
        this.tokenService.clearTokens();
        this.userService.clearCurrentUser();
        this.isAuthenticatedSubject.next(false);

        return this.router.createUrlTree(
          [this.routes.auth.login],
          returnUrl ? { queryParams: { returnUrl } } : undefined
        );
      })
    );
  }

  /** full check (sync) */
  checkAuthState(): boolean {
    return this.isAuthenticated(false);
  }

  /**
   * Refresh session once; shared across callers.
   * Returns true if refresh succeeded and tokens persisted.
   */
  private refreshSession$(): Observable<boolean> {
    if (this.refreshInFlight$) return this.refreshInFlight$;

    // Gate: if refresh token clearly invalid locally, don't call server
    if (!this.tokenService.canAttemptRefresh()) return of(false);

    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) return of(false);

    const headers = new HttpHeaders({
      [HDR.SkipError]: 'true',
      [HDR.SkipRefresh]: 'true', // avoid infinite loop
    });

    // Adjust payload/endpoint if your backend differs
    this.refreshInFlight$ = this.http
      .post<TokenPair>(this.endpoints.auth.refresh, { refreshToken }, null, { headers })
      .pipe(
        tap((tokens) => {
          if (tokens?.accessToken && tokens?.refreshToken) {
            this.tokenService.persistTokens(tokens);
            // after persist, update user state if you store user_data
            const data = localStorage.getItem('user_data');
            if (data) {
              try {
                const user = JSON.parse(data);
                this.userService.updateCurrentUser(user, tokens.accessToken);
              } catch {}
            }
            this.isAuthenticatedSubject.next(true);
          } else {
            throw new Error('Invalid refresh response');
          }
        }),
        map(() => true),
        catchError(() => of(false)),
        finalize(() => (this.refreshInFlight$ = null)),
        shareReplay(1)
      );

    return this.refreshInFlight$;
  }

  /** robust logout: clears locally even if API fails */
  logout(callServer: boolean = true): void {
    const doLocalClear = () => {
      this.tokenService.clearTokens();
      this.userService.clearCurrentUser();
      this.isAuthenticatedSubject.next(false);
      this.router.navigate([this.routes.auth.login]);
    };

    if (!callServer) {
      doLocalClear();
      return;
    }

    const headers = new HttpHeaders({
      [HDR.SkipError]: 'true',
      [HDR.SkipRefresh]: 'true',
      [HDR.LogoutFlow]: 'true',
    });

    this.http
      .post(this.endpoints.auth.logout, {}, null, { headers })
      .pipe(
        catchError(() => of(null)),
        finalize(doLocalClear)
      )
      .subscribe();
  }
}
