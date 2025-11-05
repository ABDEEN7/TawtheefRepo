import {JwtHelperService} from "@auth0/angular-jwt";
import {Injectable} from "@angular/core";

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({providedIn: 'root'})
export class TokenService {
  private jwtHelper = new JwtHelperService();
  private mem: {accessToken?: string; refreshToken?: string} = {}; // fallback

  getToken(): string | null {
    return this.safeGet('auth_token') ?? this.mem.accessToken ?? null;
  }
  getRefreshToken(): string | null {
    return this.safeGet('refresh_token') ?? this.mem.refreshToken ?? null;
  }


  persistTokens(tokens: {accessToken: string; refreshToken: string}) {
    this.tryPersistTokens(tokens);
  }

  clearTokens(): void {
    try {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('refresh_token');
    } catch {}
    this.mem = {};
  }

  decodeToken(token: string): any { return this.jwtHelper.decodeToken(token); }
  isTokenExpired(token: string): boolean { return this.jwtHelper.isTokenExpired(token); }
  getClaim(token: string, claimName: string): string | undefined {
    const decoded = this.decodeToken(token); return decoded?.[claimName];
  }
  getRoleFromToken(token: string): string {
    const decoded = this.decodeToken(token);
    return decoded?.userType ?? '';
  }

  tryPersistTokens(tokens: {accessToken: string; refreshToken: string}): boolean {
    try {
      if (tokens.accessToken) localStorage.setItem('auth_token', tokens.accessToken);
      if (tokens.refreshToken) localStorage.setItem('refresh_token', tokens.refreshToken);
      return true;
    } catch {
      this.mem.accessToken = tokens.accessToken;
      this.mem.refreshToken = tokens.refreshToken;
      return false;
    }
  }

  private safeGet(key: string): string | null {
    try { return localStorage.getItem(key); } catch { return null; }
  }
}
