import { Injectable, NgZone, OnDestroy, inject } from '@angular/core';
import { AuthService } from './auth.service';
import { MessageService } from 'primeng/api';
import { EndpointsService } from '../http/endpoints.service';
import { TranslateService } from '@ngx-translate/core';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Subject, Subscription, fromEvent, interval } from 'rxjs';
import { filter, map, takeUntil, tap } from 'rxjs/operators';

type ExternalMessageType = 'EXTERNAL_LOGIN_SUCCESS' | 'EXTERNAL_LOGIN_ERROR' | 'EXTERNAL_POPUP_CLOSED';
interface ExternalMessage {
  type: ExternalMessageType;
  userData?: any;
  content?: string;
}

@Injectable({ providedIn: 'root' })
export class ExternalLoginService implements OnDestroy {
  private readonly destroy$ = new Subject<void>();
  private popup: Window | null = null;
  private readonly popupWidth = 600;
  private readonly popupHeight = 800;

  private readonly ngZone = inject(NgZone);
  private readonly authService = inject(AuthService);
  private readonly messageService = inject(MessageService);
  private readonly endpoints = inject(EndpointsService);
  private readonly loadingService = inject(LoadingService);

  loading = false;
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
    this.loadingService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => (this.loading = state));

    // Listen for all postMessages reactively
    fromEvent<MessageEvent>(window, 'message')
      .pipe(
        takeUntil(this.destroy$),
        filter((event) => this.isAllowedOrigin(event.origin)),
        map((event) => event.data as ExternalMessage)
      )
      .subscribe((msg) => this.ngZone.run(() => this.handleMessage(msg)));
  }

  /** Starts the Azure login process */
  loginUsingAzure() {
    const url = this.endpoints.auth.externalLogin('Azure');
    this.closePopup();

    const { left, top } = this.centerPopup();
    this.popup = window.open(
      url,
      '_external_login',
      `width=${this.popupWidth},height=${this.popupHeight},left=${left},top=${top}`
    );

    if (this.safeIsPopupClosed()) {
      return this.toastWarn('Popup Blocked', 'Please allow popups for this site');
    }

    // RxJS polling to detect manual popup close
    interval(400)
      .pipe(
        takeUntil(this.destroy$),
        map(() => this.safeIsPopupClosed()),
        filter(Boolean),
        tap(() => this.closePopup())
      )
      .subscribe();
  }

  public loginUsingGoogle(): void {
    const url = this.endpoints.auth.externalLogin('google');
    this.openPopupWithState(url);
  }

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
  /** Handles all incoming messages */
  private handleMessage(msg: ExternalMessage) {
    switch (msg.type) {
      case 'EXTERNAL_LOGIN_SUCCESS':
        this.authService.externalLogin(msg.userData).subscribe({
          next: (success) =>
            success
              ? this.toastSuccess('Login Successful', 'You are now logged in')
              : this.toastError('Login Failed', 'Could not authenticate with provider'),
          error: () => this.toastError('Error', 'An error occurred during login'),
        });
        this.closePopup();
        break;

      case 'EXTERNAL_LOGIN_ERROR':
        this.toastError('Login Failed', msg.content ?? 'External authentication failed');
        this.closePopup();
        break;

      case 'EXTERNAL_POPUP_CLOSED':
        this.closePopup();
        break;
    }
  }

  /** Checks allowed origins */
  private isAllowedOrigin(origin: string) {
    const allowed = [window.location.origin, environment.apiBaseUrl];
    return allowed.includes(origin);
  }

  /** Closes popup safely */
  private closePopup() {
    if (!this.safeIsPopupClosed()) this.popup!.close();
    this.popup = null;
  }

  /** Center popup on screen */
  private centerPopup() {
    const left = window.screenX + (window.innerWidth - this.popupWidth) / 2;
    const top = window.screenY + (window.innerHeight - this.popupHeight) / 2;
    return { left, top };
  }

  /** Toast helpers */
  private toastWarn(summary: string, detail: string) {
    this.messageService.add({ severity: 'warn', summary, detail });
  }
  private toastError(summary: string, detail: string) {
    this.messageService.add({ severity: 'error', summary, detail });
  }
  private toastSuccess(summary: string, detail: string) {
    this.messageService.add({ severity: 'success', summary, detail });
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

  private toast(severity: 'success' | 'info' | 'warn' | 'error', summary: string, detail: string) {
    this.messageService.add({ severity, summary, detail });
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.closePopup();
  }
}
