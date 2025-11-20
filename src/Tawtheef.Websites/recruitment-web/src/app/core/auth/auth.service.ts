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
  private permissionsCache: Set<string> | null = null;
  constructor(
    protected core: AuthCoreService,
    protected state: AuthStateService,
    protected user: UserService,
    protected http: HttpClient,
    protected endpointService: EndpointsService,
  ) {
    this.state.checkAuthState(false);
    this.state.isAuthenticated$.subscribe(isAuth => {
      if (!isAuth) {
        this.permissionsCache = null;
      }
    });
  }

  get isAuthenticated$(): Observable<boolean> {
    return this.state.isAuthenticated$;
  }

   get isAuthenticated(): boolean {
    return this.state.isAuthenticated();
   }
   get isProfileCompleted(): boolean {
    if(!this.token) return false;
    const profile = this.decodeBootstrapFromJwt(this.token!);
    return !profile.requiresProfileCompletion;
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
  private getPermissionsFromToken(): Set<string> {
    if (this.permissionsCache) {
      return this.permissionsCache;
    }

    const token = this.token;
    if (!token) {
      this.permissionsCache = new Set<string>();
      return this.permissionsCache;
    }

    const payload = this.decodeJwtPayload<Record<string, any>>(token) ?? {};

    const result = new Set<string>();
    const permissionsArray = payload['permissions'];
    if (Array.isArray(permissionsArray)) {
      permissionsArray.forEach((p: any) => {
        if (typeof p === 'string') result.add(p);
      });
    }

    if (typeof permissionsArray === 'string') {
      permissionsArray.split(' ')
        .map(x => x.trim())
        .filter(Boolean)
        .forEach(p => result.add(p));
    }

    // 3) multiple `permission` claims (if server serializes them like that)
    const singlePermission = payload['permission'];
    if (Array.isArray(singlePermission)) {
      singlePermission.forEach((p: any) => {
        if (typeof p === 'string') result.add(p);
      });
    } else if (typeof singlePermission === 'string') {
      result.add(singlePermission);
    }

    this.permissionsCache = result;
    return result;
  }

  /**
   * Check if user has at least one / all of the required permissions.
   *
   * @param permission single permission or array
   * @param requireAll if true, all permissions must be present (AND); default: false (OR)
   */
  hasPermission(permission: string | string[], requireAll: boolean = false): boolean {
    const perms = this.getPermissionsFromToken();
    if (!permission) return false;

    const required = Array.isArray(permission) ? permission : [permission];

    if (required.length === 0) return false;
    if (perms.size === 0) return false;

    if (requireAll) {
      // AND logic
      return required.every(p => perms.has(p));
    }

    // OR logic
    return required.some(p => perms.has(p));
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
