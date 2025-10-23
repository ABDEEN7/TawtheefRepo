import {JwtHelperService} from "@auth0/angular-jwt";
import {Injectable} from "@angular/core";
import {UserType} from "../../shared/models/user-type";

interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

@Injectable({providedIn: 'root'})
export class TokenService {
  private jwtHelper = new JwtHelperService();

  constructor() {
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  persistTokens(tokens: TokenPair): void {
    if (tokens.accessToken) localStorage.setItem('auth_token', tokens.accessToken);
    if (tokens.refreshToken) localStorage.setItem('refresh_token', tokens.refreshToken);
  }

  clearTokens(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('refresh_token');
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

  getRoleFromToken(token: string): string {
    const decoded = this.decodeToken(token);
    return decoded?.userType ?? UserType.UNKNOWN;
  }
}
