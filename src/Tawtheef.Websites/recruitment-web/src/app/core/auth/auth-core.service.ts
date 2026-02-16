import { Observable, of, switchMap } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { AuthStateService } from './auth-state.service';
import { catchError, map } from 'rxjs/operators';
import { EndpointsService } from '../http/endpoints.service';
import { LoggerService } from '../services/logger.service';
import { NavigationService } from '../services/navigation.service';
import { UserInfoModel } from '../../shared/models/user-info.model';
import { AuthResponse } from '../models/auth/auth-response.model';
import { NotificationService } from '../services/notification.service';
import { TranslateService } from '@ngx-translate/core';

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
    private translate: TranslateService
  ) {}

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

    const user$ = res.user ? of(res.user) : this.loadCurrentUser();

    return user$.pipe(
      map((user) => {
        this.updateAuthState(user, accessToken);
        this.navigation.safeNavigateAfterLogin();
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
    return this.http.get<UserInfoModel>(this.endpoints.auth.me);
  }
}
