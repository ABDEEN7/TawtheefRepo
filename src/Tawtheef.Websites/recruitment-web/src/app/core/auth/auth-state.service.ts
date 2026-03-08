import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, of, shareReplay, tap, map, finalize } from 'rxjs';
import { Router, UrlTree } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

import { HttpService } from '../http/http.service';
import { EndpointsService } from '../http/endpoints.service';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { routes } from '../../routes/routes';
import { HDR } from '../utils/headers.flags';
import { ProfileStatusDto } from '../models/auth/auth-response.model';
import { TokenModel } from '../models/auth/token.model';
import { LoadingService } from '../services/loading.service';

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
    private loading: LoadingService
  ) { }

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
      } catch { }
      this.isAuthenticatedSubject.next(true);
    }

    return true;
  }

  refreshAccessToken$(): Observable<boolean> {
    if (this.refreshInFlight$) return this.refreshInFlight$;

    if (!this.tokenService.canAttemptRefresh()) {
      return of(false);
    }

    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) return of(false);

    const headers = new HttpHeaders({
      [HDR.SkipError]: 'true',
      [HDR.SkipRefresh]: 'true',
      [HDR.SkipAuth]: 'true',
    });

    this.refreshInFlight$ = this.http
      .post<TokenModel>(this.endpoints.auth.refresh, { refreshToken }, null, { headers })
      .pipe(
        tap((tokens) => {
          if (tokens?.accessToken && tokens?.refreshToken) {
            this.tokenService.persistTokens({
              accessToken: tokens.accessToken,
              refreshToken: tokens.refreshToken,
            });

            const data = localStorage.getItem('user_data');
            if (data) {
              try {
                const user = JSON.parse(data);
                this.userService.updateCurrentUser(user, tokens.accessToken);
              } catch { }
            }

            this.isAuthenticatedSubject.next(true);
            return;
          }

          throw new Error('Invalid refresh response');
        }),
        map(() => true),
        catchError(() => of(false)),
        finalize(() => {
          this.refreshInFlight$ = null;
        }),
        shareReplay(1),
      );

    return this.refreshInFlight$;
  }

  ensureAuth$(returnUrl?: string): Observable<true | UrlTree> {
    if (this.isAuthenticated(false)) return of(true);

    const token = this.tokenService.getToken();
    const hasExpiredAccess = !!token && this.tokenService.isTokenExpired(token);

    if (!hasExpiredAccess) {
      return of(
        this.router.createUrlTree(
          [this.routes.auth.login],
          returnUrl ? { queryParams: { returnUrl } } : undefined,
        ),
      );
    }

    return this.refreshAccessToken$().pipe(
      map((ok) => {
        if (ok) {
          return true as const;
        }

        this.logout(false);
        return this.router.createUrlTree(
          [this.routes.auth.login],
          returnUrl ? { queryParams: { returnUrl } } : undefined,
        );
      }),
    );
  }

  ensureAuth(returnUrl?: string): true | UrlTree {
    if (this.isAuthenticated(true)) return true;
    return this.router.createUrlTree(
      [this.routes.auth.login],
      returnUrl ? { queryParams: { returnUrl } } : undefined,
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

    this.loading.start();
    this.bootstrap$ = this.http.get<ProfileStatusDto>(this.endpoints.user.bootstrap).pipe(
      finalize(() => this.loading.stop()),
      shareReplay(1)
    );

    return this.bootstrap$;
  }

  hydrateBootstrap(data: Partial<ProfileStatusDto>): void {
    this.bootstrap$ = of(data as ProfileStatusDto).pipe(shareReplay(1));
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
        finalize(doLocalClear),
      )
      .subscribe();
  }
}
