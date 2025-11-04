import {Observable, of, switchMap} from "rxjs";
import {Injectable} from "@angular/core";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {TokenService} from "./token.service";
import {UserService} from "./user.service";
import {AuthStateService} from "./auth-state.service";
import {NavigationService} from "./navigation.service";
import {catchError, map} from "rxjs/operators";
import {AuthResponse} from "../models/auth/auth-response.model";
import {EndpointsService} from "../http/endpoints.service";
import {UserInfoModel} from "../../shared/models/user-info.model";
import {TokenModel} from "../models/auth/token.model";
import {HttpService} from "../http/http.service";

@Injectable({providedIn: 'root'})
export class AuthCoreService {
  constructor(
    private http: HttpService,
    private endpoints: EndpointsService,
    private tokenService: TokenService,
    private userService: UserService,
    private authState: AuthStateService,
    private navigation: NavigationService,
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
    if (!accessToken) return of(false);

    this.tokenService.persistTokens({
      accessToken,
      refreshToken: res.token?.refreshToken || ''
    });

    const user$ = res.user ? of(res.user) : this.loadCurrentUser();
    return user$.pipe(
      map(user => {
        this.updateAuthState(user, accessToken);
        this.navigation.navigateAfterLogin(this.tokenService.getRoleFromToken(accessToken));
        return true;
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

  register(fullName: string, email: string, gender: string, password: string, type: string): Observable<any> {
    return this.http.post<any>(this.endpoints.auth.register, {
      name: fullName,
      email,
      password,
      gender,
      userType: type
    });
  }
}
