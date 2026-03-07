import { Observable, of } from 'rxjs';
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TokenService } from './token.service';
import { UserService } from './user.service';
import { AuthStateService } from './auth-state.service';
import { catchError, map } from 'rxjs/operators';
import { EndpointsService } from '../http/endpoints.service';
import { LoggerService } from '../services/logger.service';
import { NavigationService } from '../services/navigation.service';
import { AuthResponse } from '../models/auth-response.model';
import { UserInfoModel } from '../../shared/models/user-info.model';
import { NotificationService } from '../services/notification.service';

@Injectable({ providedIn: 'root' })
export class AuthCoreService {
  private logger = inject(LoggerService);
  private notificationService = inject(NotificationService);
  private http = inject(HttpClient);
  private endpoints = inject(EndpointsService);
  private tokenService = inject(TokenService);
  private userService = inject(UserService);
  private authState = inject(AuthStateService);
  private navigation = inject(NavigationService);

  constructor() { }

  get getToken(): string | null {
    return this.tokenService.getToken();
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
      this.notificationService.error('Your browser is blocking storage. Try normal browser (not in-app/private)', 'Storage blocked');
    }

    const user$ = res.user ? of(res.user) : this.loadCurrentUser();

    return user$.pipe(
      map((user) => {
        this.updateAuthState(user, accessToken);
        this.navigation.safeNavigateAfterLogin(this.tokenService.getMainUserRole());
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
