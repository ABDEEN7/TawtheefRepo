import {JwtHelperService} from '@auth0/angular-jwt';
import {Injectable} from '@angular/core';
import {AUTH_TOKEN_KEY, OAUTH_STATE_KEY, REFRESH_TOKEN_KEY} from '../constants/auth-tokens.const';
import {SystemRoles} from '../constants/systemRoles';

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly RoleIdentifier = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
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

  getClaim(token: string, claimName: string): string | undefined {
    const decoded = this.decodeToken(token);
    return decoded?.[claimName];
  }

  public getMainUserRole(): string{
    const rawRoles = this.getRolesFromToken(this.getToken() || '');
    return rawRoles.includes(SystemRoles.SystemAdmin) ? SystemRoles.SystemAdmin
      : rawRoles.includes(SystemRoles.Employee) ? SystemRoles.Employee
        : rawRoles.includes(SystemRoles.OfficeAdmin) ? SystemRoles.OfficeAdmin
          : rawRoles.includes(SystemRoles.OfficeUser) ? SystemRoles.OfficeUser
            : '';
  }

  getRolesFromToken(token: string): string[] {
    const decoded = this.decodeToken(token);
    const roles = decoded?.[this.RoleIdentifier];
    if (Array.isArray(roles)) return roles;
    else return [roles];
  }

  tryPersistTokens(tokens: TokenPair): boolean {
    try {
      if (tokens.accessToken) {
        localStorage.setItem(AUTH_TOKEN_KEY, tokens.accessToken);
      }
      if (tokens.refreshToken) {
        localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
      }
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
