import {Observable, of, switchMap} from "rxjs";
import {Injectable} from "@angular/core";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {TokenService} from "./token.service";
import {UserService} from "./user.service";
import {AuthStateService} from "./auth-state.service";
import {catchError, map} from "rxjs/operators";
import {MessageService} from "primeng/api";
import {EndpointsService} from '../http/endpoints.service';
import {LoggerService} from '../services/logger.service';
import {NavigationService} from '../services/navigation.service';
import {AuthResponse} from '../models/auth-response.model';
import {UserInfoModel} from '../../shared/models/user-info.model';
import {TokenModel} from '../models/token.model';
import {SystemRoles} from '../constants/systemRoles';

@Injectable({providedIn: 'root'})
export class AuthCoreService {
  constructor(
    private logger: LoggerService,
    private messageService: MessageService,
    private http: HttpClient,
    private endpoints: EndpointsService,
    private tokenService: TokenService,
    private userService: UserService,
    private authState: AuthStateService,
    private navigation: NavigationService
  ) {
  }

  get getToken(): string | null {
    return this.tokenService.getToken();
  }

  externalLogin(data: AuthResponse): Observable<boolean> {
    return this.handleAuthResponse(data);
  }

  private handleAuthResponse(res: AuthResponse): Observable<boolean> {
    const accessToken = res.token?.accessToken;
    if (!accessToken) {
      this.logger.logError('Login response missing accessToken', {err: res, email: res.user?.email || ''}).subscribe();
      return of(false);
    }

    const stored = this.tokenService.tryPersistTokens({
      accessToken,
      refreshToken: res.token?.refreshToken || ''
    });
    if (!stored) {
      this.logger.logError('Failed to store tokens', {err: res, email: res.user?.email || ''}).subscribe();
      this.messageService.add({severity: 'error', summary: 'Storage blocked', detail: 'Your browser is blocking storage. Try normal browser (not in-app/private).'});
    }

    const user$ = res.user ? of(res.user) : this.loadCurrentUser();

    return user$.pipe(
      map(user => {
        this.updateAuthState(user, accessToken);
        const rawRoles = this.tokenService.getRolesFromToken(accessToken);

        const mainSystemRole =
            rawRoles.includes(SystemRoles.SystemAdmin) ? SystemRoles.SystemAdmin
          : rawRoles.includes(SystemRoles.Employee) ? SystemRoles.Employee
          : rawRoles.includes(SystemRoles.OfficeAdmin) ? SystemRoles.OfficeAdmin
          : rawRoles.includes(SystemRoles.OfficeUser) ? SystemRoles.OfficeUser
          : '';
        this.navigation.safeNavigateAfterLogin(mainSystemRole);
        return true;
      }),
      catchError(err => {
        this.logger.logError('Login failed', {err: err, email: res.user?.email || ''}).subscribe();
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

  refreshToken(): Observable<string | null> {
    if(this.authState.isAuthenticated(true)) {
      return this.http.post<TokenModel>(this.endpoints.auth.refresh, {
        accessToken: this.tokenService.getToken(),
        refreshToken: this.tokenService.getRefreshToken()
      }).pipe(
        map(response => {
          if (response.accessToken) {
            this.tokenService.persistTokens({
              accessToken: response.accessToken,
              refreshToken: response.refreshToken
            });
            return response.accessToken;
          }
          return null;
        }),
        catchError(() => {
          this.authState.logout();
          return of(null);
        })
      );
    }
    else{
      this.authState.logout();
      return of(null);
    }
  }
}
