import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import { AUTH_TOKEN_KEY, OAUTH_STATE_KEY, REFRESH_TOKEN_KEY } from '../constants/auth-tokens.const';

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  private jwtHelper = new JwtHelperService();
  private mem: Partial<TokenPair> = {}; // fallback

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
    } catch {}
    this.mem = {};
  }

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

    if (!looksLikeJwt) return null; // opaque token; server decides

    try {
      return !this.jwtHelper.isTokenExpired(token);
    } catch {
      // malformed JWT
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
   * - If returns false => don't even try, logout.
   * - If returns true/null => you may try refresh (null = opaque).
   */
  canAttemptRefresh(): boolean {
    const res = this.isRefreshTokenStillValid();
    return res !== false;
  }

  getClaim(token: string, claimName: string): string | undefined {
    const decoded = this.decodeToken(token);
    return decoded?.[claimName];
  }

  getRoleFromToken(token: string): string {
    const decoded = this.decodeToken(token);
    return decoded?.userType ?? '';
  }

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
