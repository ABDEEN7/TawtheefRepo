import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, of, shareReplay, tap, map, switchMap, finalize } from 'rxjs';
import { Router, UrlTree } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

import { HttpService } from '../http/http.service';
import { EndpointsService } from '../http/endpoints.service';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { routes } from '../../routes/routes';
import { HDR } from '../utils/headers.flags';
import { ProfileStatusDto } from '../models/auth/auth-response.model';

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  readonly routes = routes;
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  readonly isAuthenticated$ = this.isAuthenticatedSubject.asObservable();
  private bootstrap$?: Observable<ProfileStatusDto> | null;

  private refreshInFlight$?: Observable<boolean> | null;

  constructor(
    private router: Router,
    private http: HttpService,
    private endpoints: EndpointsService,
    private tokenService: TokenService,
    private userService: UserService,
  ) {}

  setAuthenticated(value: boolean): void {
    this.isAuthenticatedSubject.next(value);
  }

  isAuthenticated(ignoreExpired = false): boolean {
    const token = this.tokenService.getToken();
    const data = localStorage.getItem('user_data');

    if (!token || !data || (!ignoreExpired && this.tokenService.isTokenExpired(token))) {
      this.isAuthenticatedSubject.next(false);
      return false;
    }

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
   * Attempts refresh ONE time (shared across callers).
   * Returns true if refresh succeeded and tokens persisted.
   */
  private refreshSession$(): Observable<boolean> {
    // if there is already a refresh call running, reuse it
    if (this.refreshInFlight$) return this.refreshInFlight$;

    // quick gate: if refresh token is clearly invalid, don't call server
    if (!this.tokenService.canAttemptRefresh()) {
      return of(false);
    }

    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) return of(false);

    const headers = new HttpHeaders({
      [HDR.SkipError]: 'true',
      [HDR.SkipRefresh]: 'true', // IMPORTANT: prevent infinite loop
    });

    // ✅ adjust body/endpoint based on your backend contract
    this.refreshInFlight$ = this.http
      .post<TokenPair>(this.endpoints.auth.refresh, { refreshToken }, null, { headers })
      .pipe(
        tap((tokens) => {
          if (tokens?.accessToken && tokens?.refreshToken) {
            this.tokenService.persistTokens(tokens);
          } else {
            // backend returned unexpected payload
            throw new Error('Invalid refresh response');
          }
        }),
        map(() => true),
        catchError(() => of(false)),
        finalize(() => {
          this.refreshInFlight$ = null;
        }),
        shareReplay(1)
      );

    return this.refreshInFlight$;
  }

  /**
   * Guard-friendly: returns Observable<true|UrlTree>
   * - If access token valid => true
   * - If expired => try refresh (if possible). If refresh fails => login UrlTree
   */
  ensureAuth$(returnUrl?: string): Observable<true | UrlTree> {
    // valid access token (ignoreExpired=false here)
    if (this.isAuthenticated(false)) return of(true);

    const token = this.tokenService.getToken();
    const hasExpiredAccess = !!token && this.tokenService.isTokenExpired(token);

    if (!hasExpiredAccess) {
      // no token at all
      return of(
        this.router.createUrlTree(
          [this.routes.auth.login],
          returnUrl ? { queryParams: { returnUrl } } : undefined
        )
      );
    }

    // token expired => try refresh
    return this.refreshSession$().pipe(
      map((ok) => {
        if (ok) {
          // now we should have a new access token
          return true as const;
        }

        // refresh failed => go login
        this.tokenService.clearTokens();
        this.userService.clearCurrentUser();
        this.isAuthenticatedSubject.next(false);
        this.resetBootstrap();

        return this.router.createUrlTree(
          [this.routes.auth.login],
          returnUrl ? { queryParams: { returnUrl } } : undefined
        );
      })
    );
  }

  /** keeps your existing sync helper (but it won't refresh). */
  ensureAuth(returnUrl?: string): true | UrlTree {
    if (this.isAuthenticated(true)) return true;
    return this.router.createUrlTree(
      [this.routes.auth.login],
      returnUrl ? { queryParams: { returnUrl } } : undefined
    );
  }

  checkAuthState(allowRedirectLogout: boolean): boolean {
    const ok = this.isAuthenticated();
    if (!ok && allowRedirectLogout) this.logout(false);
    return ok;
  }

  getAuthBootstrap$(): Observable<Partial<ProfileStatusDto>> {
    if (this.bootstrap$) return this.bootstrap$;

    const token = this.tokenService.getToken();
    if (!token) {
      this.bootstrap$ = of({
        isComplete: false,
        agreedToTerms: true,
      } as ProfileStatusDto).pipe(shareReplay(1));
      return this.bootstrap$;
    }

    this.bootstrap$ = this.http
      .get<ProfileStatusDto>(this.endpoints.user.bootstrap)
      .pipe(shareReplay(1));

    return this.bootstrap$;
  }

  resetBootstrap(): void {
    this.bootstrap$ = null;
  }

  logout(callServer: boolean = true): void {
    const doLocalClear = () => {
      this.tokenService.clearTokens();
      this.userService.clearCurrentUser();
      this.isAuthenticatedSubject.next(false);
      this.resetBootstrap();
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
