import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import {
  AUTH_TOKEN_KEY,
  OAUTH_STATE_KEY,
  REFRESH_TOKEN_KEY,
} from '../constants/auth-tokens.const';

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  private jwtHelper = new JwtHelperService();
  private mem: Partial<TokenPair> = {}; // fallback

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

  /**
   * Access token validity:
   * - true only if token exists AND is a valid JWT AND not expired
   * - if access token is opaque => returns false (best practice for SPA routing decisions)
   */
  hasValidAccessToken(): boolean {
    const t = this.getToken();
    if (!t) return false;

    const res = this.isJwtTokenValid(t);
    return res === true;
  }

  /**
   * Refresh token validity for "should I attempt refresh?"
   * - JWT not expired => true
   * - opaque => null => treat as "unknown but attemptable"
   * - missing/expired => false
   */
  hasValidRefreshTokenOrUnknown(): boolean {
    const rt = this.getRefreshToken();
    if (!rt) return false;

    const res = this.isJwtTokenValid(rt);
    return res !== false; // true OR null
  }

  /**
   * Session = user_data exists AND (valid access OR refresh is valid/unknown)
   * Use this for loggedOutOnlyGuard (redirect away from /auth/*).
   */
  hasSession(): boolean {
    return (
      this.hasUserData() &&
      (this.hasValidAccessToken() || this.hasValidRefreshTokenOrUnknown())
    );
  }

  // -----------------------
  // JWT helpers
  // -----------------------
  decodeToken(token: string): any {
    try {
      return this.jwtHelper.decodeToken(token);
    } catch {
      return null;
    }
  }

  isTokenExpired(token: string): boolean {
    try {
      return this.jwtHelper.isTokenExpired(token);
    } catch {
      // malformed token => treat as expired
      return true;
    }
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

    if (!looksLikeJwt) return null; // opaque token

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
   * - null  => opaque refresh token
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
  // Claims
  // -----------------------
  getClaim(token: string, claimName: string): string | undefined {
    const decoded = this.decodeToken(token);
    const val = decoded?.[claimName];
    return typeof val === 'string' ? val : undefined;
  }

  /**
   * Keep this only if your backend truly sets `userType`.
   * Otherwise prefer role claims (you can add role parsing later).
   */
  getRoleFromToken(token: string): string {
    const decoded = this.decodeToken(token);
    const v = decoded?.userType;
    return typeof v === 'string' ? v : '';
  }

  isProfileComplete(): boolean {
    const token = this.getToken();
    if (!token) return false;

    const decoded = this.decodeToken(token);
    const val = decoded?.['profile.completed'];

    if (typeof val === 'string') {
      return val.toLowerCase() === 'true';
    }
    return !!val;
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
