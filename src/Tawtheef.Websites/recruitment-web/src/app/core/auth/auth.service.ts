import {Injectable} from "@angular/core";
import {AuthCoreService} from "./auth-core.service";
import {AuthStateService} from "./auth-state.service";
import {UserService} from "./user.service";
import {Observable, of, shareReplay} from "rxjs";
import {UserInfoModel} from "../../shared/models/user-info.model";
import {AuthBootstrap, AuthResponse} from "../models/auth/auth-response.model";
import {catchError, map} from 'rxjs/operators';
import {HttpClient} from '@angular/common/http';
import {EndpointsService} from '../http/endpoints.service';

@Injectable({providedIn: 'root'})
export class AuthService {
  private bootstrap$?: Observable<AuthBootstrap>;
  constructor(
    protected core: AuthCoreService,
    protected state: AuthStateService,
    protected user: UserService,
    protected http: HttpClient,
    protected endpointService: EndpointsService,
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

  externalLogin(data: AuthResponse) {
    return this.core.externalLogin(data);
  }

  logout(callServer: boolean = true): void {
    this.state.logout(callServer);
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

  getAuthBootstrap$(): Observable<AuthBootstrap> {
    if (this.bootstrap$) return this.bootstrap$;

    // Not logged in → no need to fetch
    const token = this.token;
    if (!token) {
      this.bootstrap$ = of({
        requiresProfileCompletion: false,
        missingFields: [],
        prefill: null
      }).pipe(shareReplay(1));
      return this.bootstrap$;
    }

    this.bootstrap$ = this.http.get<Partial<AuthResponse>>(this.endpointService.user.bootstrap).pipe(
      map(resp => {
        const requires = !!resp?.requiresProfileCompletion;
        const missing = (resp?.missingFields ?? []) as string[];
        const prefill = (resp?.prefill ?? null) as AuthBootstrap['prefill'];

        return {
          requiresProfileCompletion: requires,
          missingFields: missing,
          prefill
        } satisfies AuthBootstrap;
      }),
      catchError(() => of(this.decodeBootstrapFromJwt(token))),
      shareReplay(1)
    );

    return this.bootstrap$;
  }

  // ----------------- helpers -----------------

  /** Safe decode for JWT payload (no atob Unicode issues) */
  private decodeJwtPayload<T = any>(jwt: string): T | null {
    try {
      const payload = jwt.split('.')[1];
      if (!payload) return null;
      // base64url → base64
      const b64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(b64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(json) as T;
    } catch {
      return null;
    }
  }

  /** Build a minimal bootstrap view from token claims */
  private decodeBootstrapFromJwt(token: string): AuthBootstrap {
    const payload = this.decodeJwtPayload<Record<string, any>>(token) ?? {};
    // Your server issues claim "profile.completed" = "true" | "false"
    const completedRaw = payload['profile.completed'];
    const completed = (typeof completedRaw === 'string')
      ? completedRaw.toLowerCase() === 'true'
      : !!completedRaw;

    return {
      requiresProfileCompletion: !completed,
      missingFields: [], // we can’t know without server; leave empty
      prefill: null       // also requires server (claims snapshot) — optional to enhance by embedding in JWT
    };
  }
}
