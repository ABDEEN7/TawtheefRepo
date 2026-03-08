import { Observable, of, switchMap } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { AuthStateService } from './auth-state.service';
import { catchError, finalize, map } from 'rxjs/operators';
import { EndpointsService } from '../http/endpoints.service';
import { LoggerService } from '../services/logger.service';
import { NavigationService } from '../services/navigation.service';
import { UserInfoModel } from '../../shared/models/user-info.model';
import { AuthResponse } from '../models/auth/auth-response.model';
import { NotificationService } from '../services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { LoadingService } from '../services/loading.service';

@Injectable({ providedIn: 'root' })
export class AuthCoreService {
  constructor(
    private logger: LoggerService,
    private http: HttpClient,
    private endpoints: EndpointsService,
    private tokenService: TokenService,
    private userService: UserService,
    private authState: AuthStateService,
    private navigation: NavigationService,
    private notifier: NotificationService,
    private translate: TranslateService,
    private loading: LoadingService
  ) { }

  get getToken(): string | null {
    return this.tokenService.getToken();
  }

  login(email: string, password: string): Observable<boolean> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http
      .post<AuthResponse>(this.endpoints.auth.login, { email, password }, { headers })
      .pipe(
        switchMap((res) => this.handleAuthResponse(res)),
        catchError((err) => {
          this.logger.logError('Login failed', { err: err, email: email }).subscribe();
          return of(false);
        })
      );
  }

  externalLogin(data: AuthResponse): Observable<boolean> {
    return this.handleAuthResponse(data);
  }

  private user$ = (res: AuthResponse) => res.user ? of(res.user) : this.loadCurrentUser();

  private handleAuthResponse(res: AuthResponse): Observable<boolean> {
    const accessToken = res.token?.accessToken;
    if (!accessToken) {
      this.logger.logError('Login response missing accessToken', { err: res, email: res.user?.email || '' }).subscribe();
      return of(false);
    }

    const stored = this.tokenService.tryPersistTokens({
      accessToken,
      refreshToken: res.token?.refreshToken || ''
    });

    if (!stored) {
      this.logger.logError('Failed to store tokens', { err: res, email: res.user?.email || '' }).subscribe();
      this.notifier.error(
        this.translate.instant('auth.storageBlocked.detail'),
        this.translate.instant('auth.storageBlocked.summary')
      );
    }

    return this.user$(res).pipe(
      map((user) => {
        this.updateAuthState(user as UserInfoModel, accessToken);
        this.navigation.safeNavigateAfterLogin(res.requiresProfileCompletion);
        return true;
      }),
      catchError((err) => {
        this.logger.logError('Login failed', { err: err, email: res.user?.email || '' }).subscribe();
        return of(false);
      })
    );
  }

  private updateAuthState(user: UserInfoModel, accessToken: string): void {
    this.userService.updateCurrentUser(user, accessToken);
    this.authState.setAuthenticated(true);
  }

  private loadCurrentUser(): Observable<UserInfoModel> {
    this.loading.start();
    return this.http.get<UserInfoModel>(this.endpoints.auth.me).pipe(
      finalize(() => this.loading.stop())
    );
  }
}
