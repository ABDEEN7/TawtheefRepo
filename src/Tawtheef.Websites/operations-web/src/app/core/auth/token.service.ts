import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import {
  AUTH_TOKEN_KEY,
  OAUTH_STATE_KEY,
  REFRESH_TOKEN_KEY,
} from '../constants/auth-tokens.const';
import { SystemRoles } from '../constants/systemRoles';

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  private jwtHelper = new JwtHelperService();
  private mem: Partial<TokenPair> = {}; // fallback

  private readonly RoleIdentifier =
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  // -----------------------
  // Tokens
  // -----------------------
  getToken(): string | null {
    return this.safeGet(AUTH_TOKEN_KEY) ?? this.mem.accessToken ?? null;
  }

  getRefreshToken(): string | null {
    return this.safeGet(REFRESH_TOKEN_KEY) ?? this.mem.refreshToken ?? null;
  }

  persistTokens(tokens: TokenPair): void {
    this.tryPersistTokens(tokens);
  }

  clearTokens(): void {
    try {
      localStorage.removeItem(AUTH_TOKEN_KEY);
      localStorage.removeItem(REFRESH_TOKEN_KEY);
      localStorage.removeItem(OAUTH_STATE_KEY);
    } catch { }
    this.mem = {};
  }

  // -----------------------
  // Session helpers (NEW)
  // -----------------------
  hasUserData(): boolean {
    try {
      return !!localStorage.getItem('user_data');
    } catch {
      return false;
    }
  }

  /** access token present AND (if JWT) not expired */
  hasValidAccessToken(): boolean {
    const t = this.getToken();
    if (!t) return false;

    const res = this.isJwtTokenValid(t);
    // If token is opaque (null), treat as NOT valid for auth decisions
    // (you can change this to true if your access tokens are never opaque)
    if (res === null) return false;

    return res;
  }

  /**
   * refresh token present AND:
   * - if JWT: not expired
   * - if opaque: unknown -> treat as "attemptable" (true for refresh attempt decisions)
   */
  hasValidRefreshTokenOrUnknown(): boolean {
    const rt = this.getRefreshToken();
    if (!rt) return false;

    const res = this.isJwtTokenValid(rt);
    return res !== false; // true OR null => ok to attempt refresh
  }

  /**
   * “Has session” means:
   * user_data exists AND (valid access OR refresh is valid/unknown)
   * This is what you should use for loggedOutOnlyGuard redirecting away from /auth/*
   */
  hasSession(): boolean {
    return this.hasUserData() && (this.hasValidAccessToken() || this.hasValidRefreshTokenOrUnknown());
  }

  // -----------------------
  // JWT helpers
  // -----------------------
  decodeToken(token: string): any {
    return this.jwtHelper.decodeToken(token);
  }

  isTokenExpired(token: string): boolean {
    return this.jwtHelper.isTokenExpired(token);
  }

  /**
   * Returns:
   * - true  => token is a JWT and NOT expired
   * - false => token is a JWT and expired (or invalid JWT)
   * - null  => token is not a JWT (opaque), can't be checked client-side
   */
  isJwtTokenValid(token: string): boolean | null {
    if (!token) return false;

    const parts = token.split('.');
    const looksLikeJwt = parts.length === 3;

    if (!looksLikeJwt) return null;

    try {
      return !this.jwtHelper.isTokenExpired(token);
    } catch {
      return false;
    }
  }

  /**
   * Best-effort refresh token validity:
   * - false => missing/expired/malformed (if JWT)
   * - true  => seems valid (JWT not expired)
   * - null  => opaque refresh token (can't know locally)
   */
  isRefreshTokenStillValid(): boolean | null {
    const rt = this.getRefreshToken();
    if (!rt) return false;
    return this.isJwtTokenValid(rt);
  }

  /**
   * Useful when deciding whether to attempt refresh:
   * - false => don't try
   * - true/null => can try (null = opaque)
   */
  canAttemptRefresh(): boolean {
    return this.isRefreshTokenStillValid() !== false;
  }

  // -----------------------
  // Claims / roles (hardened)
  // -----------------------
  getClaim(token: string, claimName: string): string | undefined {
    const decoded = this.decodeToken(token);
    return decoded?.[claimName];
  }

  getRoleFromToken(token: string): string {
    const roles = this.getRolesFromToken(token);
    if (roles.length) return roles[0]; // or your own priority logic
    return '';
  }

  public getMainUserRole(): string {
    const rawRoles = this.getRolesFromToken(this.getToken() || '');
    if (rawRoles.includes(SystemRoles.SystemAdmin)) return SystemRoles.SystemAdmin;
    if (rawRoles.includes(SystemRoles.HrManager)) return SystemRoles.HrManager;
    if (rawRoles.includes(SystemRoles.DepartmentManager)) return SystemRoles.DepartmentManager;
    if (rawRoles.includes(SystemRoles.OfficeAdmin)) return SystemRoles.OfficeAdmin;
    if (rawRoles.includes(SystemRoles.OfficeUser)) return SystemRoles.OfficeUser;
    if (rawRoles.includes(SystemRoles.Employee)) return SystemRoles.Employee;
    return '';
  }

  getRolesFromToken(token: string): string[] {
    if (!token) return [];

    let decoded: any;
    try {
      decoded = this.decodeToken(token);
    } catch {
      return [];
    }

    const roles = decoded?.[this.RoleIdentifier];

    if (Array.isArray(roles)) {
      return roles.filter((r) => typeof r === 'string' && r.trim().length > 0);
    }

    if (typeof roles === 'string' && roles.trim().length > 0) return [roles];

    return [];
  }

  // -----------------------
  // Persistence
  // -----------------------
  tryPersistTokens(tokens: TokenPair): boolean {
    try {
      if (tokens.accessToken) localStorage.setItem(AUTH_TOKEN_KEY, tokens.accessToken);
      if (tokens.refreshToken) localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
      return true;
    } catch {
      this.mem.accessToken = tokens.accessToken;
      this.mem.refreshToken = tokens.refreshToken;
      return false;
    }
  }

  private safeGet(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  }
}
