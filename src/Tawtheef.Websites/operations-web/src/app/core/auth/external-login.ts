import { Injectable, NgZone, OnDestroy, inject } from '@angular/core';
import { AuthService } from './auth.service';
import { MessageService } from 'primeng/api';
import { EndpointsService } from '../http/endpoints.service';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../../environments/environment';
import { Subject, fromEvent, interval } from 'rxjs';
import { filter, map, takeUntil, tap } from 'rxjs/operators';
import {NotificationService} from '../services/notification.service';
import {TranslateService} from '@ngx-translate/core';

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
  private readonly notificationService = inject(NotificationService);
  private readonly endpoints = inject(EndpointsService);
  private readonly loadingService = inject(LoadingService);
  private readonly translate = inject(TranslateService);

  loading = false;

  // i18n keys (service-local)
  private readonly i18n = {
    popupBlockedSummary: 'auth.externalLogin.popupBlocked.summary',
    popupBlockedDetailShort: 'auth.externalLogin.popupBlocked.detailShort',
    popupBlockedDetailContinue: 'auth.externalLogin.popupBlocked.detailContinue',

    loginSuccessfulSummary: 'auth.externalLogin.loginSuccessful.summary',
    loginSuccessfulDetail: 'auth.externalLogin.loginSuccessful.detail',

    loginFailedSummary: 'auth.externalLogin.loginFailed.summary',
    loginFailedDetailProvider: 'auth.externalLogin.loginFailed.detailProvider',

    authErrorSummary: 'auth.externalLogin.authError.summary',
    authErrorDetailLogin: 'auth.externalLogin.authError.detailLogin',

    externalAuthFailedDetailFallback: 'auth.externalLogin.externalAuthFailed.detailFallback',
  } as const;

  private i18nText(key: string): string {
    return this.translate.instant(key);
  }
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
      return this.toastKey('warn', this.i18n.popupBlockedSummary, this.i18n.popupBlockedDetailShort);
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
      this.toastKey('warn', this.i18n.popupBlockedSummary, this.i18n.popupBlockedDetailContinue);
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
              ? this.toastKey('success', this.i18n.loginSuccessfulSummary, this.i18n.loginSuccessfulDetail)
              : this.toastKey('error', this.i18n.loginFailedSummary, this.i18n.loginFailedDetailProvider),
          error: () => this.toastKey('error', this.i18n.authErrorSummary, this.i18n.authErrorDetailLogin),
        });
        this.closePopup();
        break;

      case 'EXTERNAL_LOGIN_ERROR':
        // If msg.content is localized (sent by backend/popup), pass it through; otherwise fallback to key.
        this.toast(
          'error',
          this.i18nText(this.i18n.loginFailedSummary),
          msg.content ?? this.i18nText(this.i18n.externalAuthFailedDetailFallback)
        );
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

  /** Toast helpers (key-based) */
  private toastKey(
    severity: 'success' | 'info' | 'warn' | 'error',
    summaryKey: string,
    detailKey: string
  ) {
    switch (severity) {
      case 'success':
        this.notificationService.success(this.i18nText(summaryKey), this.i18nText(detailKey));
        break;
      case 'info':
        this.notificationService.info(this.i18nText(summaryKey), this.i18nText(detailKey));
        break;
      case 'warn':
        this.notificationService.warn(this.i18nText(summaryKey), this.i18nText(detailKey));
        break;
      case 'error':
        this.notificationService.error(this.i18nText(summaryKey), this.i18nText(detailKey));
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

  private toast(severity: 'success' | 'info' | 'warn' | 'error', summary: string, detail: string) {
    switch (severity) {
      case 'success':
        this.notificationService.success(summary, detail);
        break;
      case 'info':
        this.notificationService.info(summary, detail);
        break;
      case 'warn':
        this.notificationService.warn(summary, detail);
        break;
      case 'error':
        this.notificationService.error(summary, detail);
        break;
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.closePopup();
  }
}
