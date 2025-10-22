import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EndpointsService } from '../http/endpoints.service';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, throwError, from } from 'rxjs';
import { tap, catchError, switchMap, finalize } from 'rxjs/operators';

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  expiresAt?: number; // optional unix ms timestamp
}

export interface UserProfile {
  id?: string;
  name?: string;
  email?: string;
  // add other claims you expect
}

const STORAGE_KEYS = {
  tokens: 'app_tokens_v1',
  user: 'app_user_v1'
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokens: AuthTokens | null = null;
  private userSubject = new BehaviorSubject<UserProfile | null>(null);
  user$ = this.userSubject.asObservable();

  // single-flight refresh guard
  private refreshingInProgress = false;
  private refreshPromise: Promise<AuthTokens | null> | null = null;

  constructor(
    private http: HttpClient,
    private router: Router,
    private endpoints: EndpointsService
  ) {
    this.loadFromStorage();
  }

  /* --------------------------
     External login initiation
     providers: 'google' | 'microsoft' | 'azure' | 'sso'
     ---------------------------*/
  startExternalLogin(provider: 'google' | 'microsoft' | 'azure' | 'sso', usePopup = false) {
    const url = this.endpoints.auth.externalLogin(provider);
    if (!usePopup) {
      // redirect (simpler)
      window.location.href = url;
      return;
    }

    // popup flow (optional)
    const width = 600, height = 700;
    const left = (screen.width / 2) - (width / 2);
    const top = (screen.height / 2) - (height / 2);
    const popup = window.open(url, 'oauthPopup', `width=${width},height=${height},left=${left},top=${top}`);

    // optional: listen for message from popup (backend must postMessage to parent on success)
    // Here we return nothing; consumer may rely on callback route flow.
  }

  /* --------------------------
     Called on callback route after provider redirects back with ?code=...&state=...
     exchange the code with backend to obtain tokens
     ---------------------------*/
  finishLoginByCode(provider: string, code: string, state?: string): Observable<AuthTokens> {
    // backend should accept provider + code and return tokens + optional userModel profile
    return this.http.post<{ tokens: AuthTokens; user?: UserProfile }>(
      this.endpoints.auth.exchangeCode(),
      { provider, code, state }
    ).pipe(
      tap(resp => {
        if (resp && resp.tokens) {
          this.setTokens(resp.tokens);
          if (resp.user) this.setUser(resp.user);
        }
      }),
      switchMap(resp => of(resp.tokens)),
      catchError(err => {
        console.error('finishLogin error', err);
        return throwError(() => err);
      })
    );
  }

  /* --------------------------
     Direct login with credentials (optional)
     ---------------------------*/
  loginWithPassword(payload: { username: string; password: string }): Observable<AuthTokens> {
    return this.http.post<{ tokens: AuthTokens; user?: UserProfile }>(
      this.endpoints.auth.login(), payload
    ).pipe(
      tap(resp => {
        if (resp?.tokens) this.setTokens(resp.tokens);
        if (resp?.user) this.setUser(resp.user);
      }),
      switchMap(resp => of(resp.tokens))
    );
  }

  /* --------------------------
     Refresh token logic — returns Promise that resolves to new tokens or null
     single-flight: only one refresh in progress at a time
     ---------------------------*/
  refreshToken(): Promise<AuthTokens | null> {
    if (!this.tokens?.refreshToken) {
      return Promise.resolve(null);
    }

    if (this.refreshPromise) {
      return this.refreshPromise; // return inflight
    }

    this.refreshingInProgress = true;
    this.refreshPromise = this.http.post<{ tokens: AuthTokens; user?: UserProfile }>(
      this.endpoints.auth.refresh(),
      { refreshToken: this.tokens.refreshToken }
    ).pipe(
      tap(resp => {
        if (resp?.tokens) {
          this.setTokens(resp.tokens);
          if (resp.user) this.setUser(resp.user);
        }
      }),
      switchMap(resp => of(resp.tokens)),
      catchError(err => {
        // refresh failed -> logout locally
        console.warn('refresh failed', err);
        this.clearSession();
        return of(null);
      }),
      finalize(() => {
        this.refreshingInProgress = false;
        this.refreshPromise = null;
      })
    ).toPromise();

    return this.refreshPromise;
  }

  /* --------------------------
     Get current access token (string)
     ---------------------------*/
  getAccessToken(): string | null {
    return this.tokens?.accessToken ?? null;
  }

  /* --------------------------
     Set tokens into memory + storage
     ---------------------------*/
  setTokens(tokens: AuthTokens) {
    this.tokens = tokens;
    try {
      localStorage.setItem(STORAGE_KEYS.tokens, JSON.stringify(tokens));
    } catch { /* ignore */ }
  }

  /* --------------------------
     Set userModel profile
     ---------------------------*/
  setUser(user: UserProfile) {
    this.userSubject.next(user);
    try {
      localStorage.setItem(STORAGE_KEYS.user, JSON.stringify(user));
    } catch {}
  }

  /* --------------------------
     Load from storage on startup
     ---------------------------*/
  private loadFromStorage() {
    try {
      const raw = localStorage.getItem(STORAGE_KEYS.tokens);
      if (raw) this.tokens = JSON.parse(raw);
      const u = localStorage.getItem(STORAGE_KEYS.user);
      if (u) this.userSubject.next(JSON.parse(u));
    } catch {
      this.tokens = null;
      this.userSubject.next(null);
    }
  }

  /* --------------------------
     Logout: call backend, clear storage, redirect optionally to provider logout
     providerLogoutUrl optional: if backend returns it or you compute it
     ---------------------------*/
  logout(redirectTo = '/login'): Observable<any> {
    // Try to notify server to revoke refresh token
    return this.http.post(this.endpoints.auth.logout(), { refreshToken: this.tokens?.refreshToken ?? null }).pipe(
      tap(() => {
        this.clearSession();
        // redirect to app login page
        this.router.navigateByUrl(redirectTo);
      }),
      catchError(err => {
        // still clear local session even if server fails
        console.warn('logout error', err);
        this.clearSession();
        this.router.navigateByUrl(redirectTo);
        return of(null);
      })
    );
  }

  /* --------------------------
     Clear local session
     ---------------------------*/
  clearSession() {
    this.tokens = null;
    this.userSubject.next(null);
    try {
      localStorage.removeItem(STORAGE_KEYS.tokens);
      localStorage.removeItem(STORAGE_KEYS.user);
    } catch {}
  }

  /* --------------------------
     Helper: check if access token near expiry (optional)
     you can implement token decode to check 'exp' claim
     ---------------------------*/
  isAuthenticated(): boolean {
    return !!this.tokens?.accessToken;
  }
}
