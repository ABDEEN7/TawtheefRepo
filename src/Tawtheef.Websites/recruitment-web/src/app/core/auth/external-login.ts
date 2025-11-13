import { Injectable, NgZone, OnDestroy, inject } from '@angular/core';
import { AuthService } from './auth.service';
import { MessageService } from 'primeng/api';
import { EndpointsService } from '../http/endpoints.service';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../../environments/environment';
import { Subject, fromEvent, interval, of } from 'rxjs';
import { catchError, filter, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import {AuthResponse} from '../models/auth/auth-response.model';
import {ExternalMsg} from '../../pages/auth/popup-callback/popup-callback';

@Injectable({ providedIn: 'root' })
export class ExternalLoginService implements OnDestroy {
  private readonly destroy$ = new Subject<void>();

  private readonly popupWidth = 600;
  private readonly popupHeight = 800;
  private popup: Window | null = null;

  private readonly ngZone = inject(NgZone);
  private readonly authService = inject(AuthService);
  private readonly messageService = inject(MessageService);
  private readonly endpoints = inject(EndpointsService);
  private readonly loadingService = inject(LoadingService);

  loading = false;

  // allow-list the origins that are permitted to postMessage back
  private readonly allowedOrigins = new Set<string>([
    window.location.origin,
    environment.apiBaseUrl
  ]);

  private safeIsPopupClosed(): boolean {
    try {
      // Accessing .closed can throw under COOP when popup is cross-origin
      return !this.popup || this.popup.closed;
    } catch {
      // Treat as "not closed" and let postMessage or timeout handle the flow
      return false;
    }
  }
  constructor() {
    // mirror loading flag
    this.loadingService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe(v => (this.loading = v));

    // reactively handle postMessage events
    fromEvent<MessageEvent>(window, 'message')
      .pipe(
        takeUntil(this.destroy$),
        filter(evt => this.allowedOrigins.has(evt.origin)),
        map(evt => evt.data as ExternalMsg),
      )
      .subscribe(msg => this.ngZone.run(() => this.handleMessage(msg)));
  }

  /** Public APIs */
  public loginUsingGoogle(): void {
    const url = this.endpoints.auth.externalLogin('google');
    this.openPopupWithState(url);
  }

  public loginUsingQatarPass(): void {
    const url = environment.qatarPassLoginUrl;
    this.openPopupWithState(url);
  }

  /** Open popup (centered), add `state`, and start polling for manual close */
  private openPopupWithState(baseUrl: string): void {
    this.closePopup();

    const state = Math.random().toString(36).slice(2);
    localStorage.setItem('oauth_state', state);
    const url = baseUrl.includes('?') ? `${baseUrl}&state=${state}` : `${baseUrl}?state=${state}`;

    const { left, top } = this.centeredPosition();
    this.popup = window.open(
      url,
      '_external_login',
      `width=${this.popupWidth},height=${this.popupHeight},left=${left},top=${top},resizable=yes,scrollbars=yes`
    );

    if (this.safeIsPopupClosed()) {
      this.toast('warn', 'Popup Blocked', 'Please allow popups for this site to continue with external login');
      return;
    }

    // Fallback: detect manual close via polling
    interval(350)
      .pipe(
        takeUntil(this.destroy$),
        map(() => this.safeIsPopupClosed()),
        filter(Boolean),
        tap(() => this.closePopup())
      )
      .subscribe();
  }

  /** Handle messages from popup */
  private handleMessage(msg: ExternalMsg): void {
    switch (msg.type) {
      case 'EXTERNAL_LOGIN_SUCCESS':
        of(msg.userData)
          .pipe(
            switchMap(user => this.authService.externalLogin(user!)),
            catchError(() => {
              this.toast('error', 'Error', 'An error occurred during authentication');
              return of(false);
            })
          )
          .subscribe(success => {
            if (!success) {
              this.toast('error', 'Login Failed', 'Could not authenticate with external provider');
            }
          });
        this.closePopup();
        break;

      case 'EXTERNAL_LOGIN_ERROR':
        this.toast('error', 'Login Failed', msg.content ?? 'External authentication failed');
        this.closePopup();
        break;

      case 'EXTERNAL_POPUP_CLOSED':
        this.closePopup();
        break;

      default:
        // ignore unknown messages
        break;
    }
  }

  /** Utilities */
  private centeredPosition() {
    // robust centering across multi-monitor setups
    const dualLeft = (window.screenLeft ?? window.screenX ?? 0);
    const dualTop = (window.screenTop ?? window.screenY ?? 0);
    const width = window.innerWidth || document.documentElement.clientWidth || screen.width;
    const height = window.innerHeight || document.documentElement.clientHeight || screen.height;
    const left = Math.max(0, dualLeft + (width - this.popupWidth) / 2);
    const top = Math.max(0, dualTop + (height - this.popupHeight) / 2);
    return { left, top };
  }

  private closePopup(): void {
    if (!this.safeIsPopupClosed()) this.popup!.close();
    this.popup = null;
  }

  private toast(severity: 'success' | 'info' | 'warn' | 'error', summary: string, detail: string) {
    this.messageService.add({ severity, summary, detail });
  }

  /** teardown */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.closePopup();
  }
}
