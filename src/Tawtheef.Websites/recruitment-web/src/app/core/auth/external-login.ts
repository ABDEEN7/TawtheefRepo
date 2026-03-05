import { inject, Injectable, NgZone, OnDestroy } from '@angular/core';
import { AuthService } from './auth.service';
import { EndpointsService } from '../http/endpoints.service';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../../environments/environment';
import { fromEvent, interval, merge, of, Subject, Subscription, timer } from 'rxjs';
import { catchError, filter, finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { ExternalMsg } from '../../pages/auth/popup-callback/popup-callback';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService } from '../services/notification.service';
import { OAUTH_STATE_KEY } from '../constants/auth-tokens.const';

type ExternalProvider = 'google' | 'qatarpass';

@Injectable({ providedIn: 'root' })
export class ExternalLoginService implements OnDestroy {
  private readonly destroy$ = new Subject<void>();

  private readonly popupWidth = 600;
  private readonly popupHeight = 800;
  private readonly popupTimeoutMs = 60_000 * 5;

  private popup: Window | null = null;
  private popupDone$: Subject<void> | null = null;
  private popupPollSub: Subscription | null = null;
  private popupTimeoutSub: Subscription | null = null;
  private popupFocusSub: Subscription | null = null;
  private isAccessRestricted = false;

  private readonly ngZone = inject(NgZone);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly endpoints = inject(EndpointsService);
  private readonly loadingService = inject(LoadingService);
  private readonly translate = inject(TranslateService);

  /** UI State consumed by components */
  loading = false;
  popupStatusKey: string | null = null;

  /** Whether we can show Retry / Open in new tab actions */
  canRetry = false;
  canOpenInNewTab = false;

  private externalLoadingActive = false;
  private readonly allowedOrigins = this.buildAllowedOrigins();

  /** Remember last attempt so UI can retry/open tab */
  private lastBaseUrl: string | null = null;
  private lastFullUrl: string | null = null;
  private lastProvider: ExternalProvider | null = null;

  // i18n keys (service-local)
  private readonly i18n = {
    popupBlockedSummary: 'auth.externalLogin.popupBlocked.summary',
    popupBlockedDetail: 'auth.externalLogin.popupBlocked.detail',

    authErrorSummary: 'auth.externalLogin.authError.summary',
    authErrorDetail: 'auth.externalLogin.authError.detail',

    loginFailedSummary: 'auth.externalLogin.loginFailed.summary',
    loginFailedDetail: 'auth.externalLogin.loginFailed.detail',

    externalAuthFailedSummary: 'auth.externalLogin.externalAuthFailed.summary',
    externalAuthFailedDetailFallback: 'auth.externalLogin.externalAuthFailed.detailFallback',

    popupClosedSummary: 'auth.externalLogin.popupClosed.summary',
    popupClosedDetail: 'auth.externalLogin.popupClosed.detail',

    timeoutSummary: 'auth.externalLogin.timeout.summary',
    timeoutDetail: 'auth.externalLogin.timeout.detail',

    loadingOpening: 'auth.externalLogin.loadingOpening',
    loadingWaitingUser: 'auth.externalLogin.loadingWaitingUser', // NEW (recommended)
    loadingCompleting: 'auth.externalLogin.loadingCompleting',

    hintReturnToApp: 'auth.externalLogin.hintReturnToApp', // NEW (recommended)
  } as const;

  constructor() {
    fromEvent<MessageEvent>(window, 'message')
      .pipe(
        takeUntil(this.destroy$),
        filter((evt) => this.allowedOrigins.includes(evt.origin)),
        map((evt) => evt.data as ExternalMsg),
      )
      .subscribe((msg) => this.ngZone.run(() => this.handleMessage(msg)));
  }

  /** Public APIs */
  public loginUsingGoogle(): void {
    const baseUrl = this.endpoints.auth.externalLogin('google');
    this.lastProvider = 'google';
    this.lastBaseUrl = baseUrl;
    this.openPopupWithState(baseUrl);
  }

  public loginUsingQatarPass(): void {
    const baseUrl = environment.qatarPassLoginUrl;
    this.lastProvider = 'qatarpass';
    this.lastBaseUrl = baseUrl;
    this.openPopupWithState(baseUrl);
  }

  /** NEW: allow component to cancel ongoing flow */
  public cancelExternalLogin(): void {
    this.finishPopupFlow({ stopLoading: true, closePopup: true });
    this.setActions({ retry: !!this.lastBaseUrl, openTab: !!this.lastFullUrl });
  }

  /** NEW: retry the last provider attempt (popup) */
  public retryLast(): void {
    if (!this.lastBaseUrl) return;
    this.openPopupWithState(this.lastBaseUrl);
  }

  /** NEW: open last auth URL in a new tab (fallback when popup blocked) */
  public openLastInNewTab(): void {
    if (!this.lastFullUrl) return;
    // try new tab
    try {
      window.open(this.lastFullUrl, '_blank', 'noopener,noreferrer');
    } catch {
      // last resort: same tab
      window.location.href = this.lastFullUrl;
    }
  }

  private i18nText(key: string): string {
    return this.translate.instant(key);
  }

  private safeIsPopupClosed(): boolean {
    if (!this.popup) return true;

    try {
      return this.popup.closed;
    } catch {
      // If browser blocks access, we can't reliably detect close
      this.isAccessRestricted = true;
      return false;
    }
  }

  private isCrossChain(url: string): boolean {
    try {
      const target = new URL(url, window.location.origin);
      return target.origin !== window.location.origin;
    } catch {
      return true; // Assume cross-origin if URL is malformed
    }
  }

  /** Open popup (centered), add `state`, and start guarded fallback listeners */
  private openPopupWithState(baseUrl: string): void {
    // when user starts, reset actions until we know outcome
    this.setActions({ retry: false, openTab: false });

    const state = this.generateState();
    localStorage.setItem(OAUTH_STATE_KEY, state);

    const fullUrl = baseUrl.includes('?') ? `${baseUrl}&state=${state}` : `${baseUrl}?state=${state}`;
    this.lastFullUrl = fullUrl;

    this.startNewPopupFlow(fullUrl, this.centeredPosition());
  }

  private startNewPopupFlow(url: string, position: { left: number; top: number }): void {
    this.finishPopupFlow({ stopLoading: true, closePopup: true });
    this.popupDone$ = new Subject<void>();

    this.setLoadingState(true, this.i18n.loadingOpening);

    this.popup = window.open(
      url,
      '_external_login',
      `width=${this.popupWidth},height=${this.popupHeight},left=${position.left},top=${position.top},resizable=yes,scrollbars=yes`
    );

    // blocked
    if (this.safeIsPopupClosed()) {
      this.finishPopupFlow({ stopLoading: true, closePopup: true });
      this.toastKey('warn', this.i18n.popupBlockedSummary, this.i18n.popupBlockedDetail);

      // Enable UI actions (Retry + Open in new tab)
      this.setActions({ retry: true, openTab: true });
      return;
    }

    // waiting for user to finish in provider
    this.setLoadingState(true, this.i18n.loadingWaitingUser);

    const untilPopupDone$ = merge(this.destroy$, this.popupDone$);

    this.isAccessRestricted = this.isCrossChain(url);

    // If cross-origin restricted, polling close may be unreliable.
    // Show a helpful hint to user.
    if (this.isAccessRestricted) {
      this.popupStatusKey = this.i18n.hintReturnToApp;
    }

    this.popupPollSub = interval(700)
      .pipe(takeUntil(untilPopupDone$))
      .subscribe(() => {
        if (this.safeIsPopupClosed()) {
          this.finishPopupFlow({ stopLoading: true, closePopup: true });
          this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
          this.setActions({ retry: true, openTab: !!this.lastFullUrl });
        }
      });

    this.popupTimeoutSub = timer(this.popupTimeoutMs)
      .pipe(takeUntil(untilPopupDone$))
      .subscribe(() => {
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        this.toastKey('warn', this.i18n.timeoutSummary, this.i18n.timeoutDetail);

        this.setActions({ retry: true, openTab: !!this.lastFullUrl });
      });

    this.popupFocusSub = fromEvent(window, 'focus')
      .pipe(takeUntil(untilPopupDone$))
      .subscribe(() => {
        if (this.safeIsPopupClosed()) {
          this.finishPopupFlow({ stopLoading: true, closePopup: true });
          this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
          this.setActions({ retry: true, openTab: !!this.lastFullUrl });
        }
      });
  }

  private finishPopupFlow(options: { stopLoading: boolean; closePopup: boolean }): void {
    if (this.popupDone$) {
      this.popupDone$.next();
      this.popupDone$.complete();
      this.popupDone$ = null;
    }

    this.popupPollSub?.unsubscribe();
    this.popupPollSub = null;

    this.popupTimeoutSub?.unsubscribe();
    this.popupTimeoutSub = null;

    this.popupFocusSub?.unsubscribe();
    this.popupFocusSub = null;

    if (options.closePopup) {
      this.closePopup();
    }

    if (options.stopLoading) {
      this.setLoadingState(false);
    }
  }

  /** Handle messages from popup */
  private handleMessage(msg: ExternalMsg): void {
    if (this.loading && msg.type === 'EXTERNAL_LOGIN_SUCCESS') return;

    switch (msg.type) {
      case 'EXTERNAL_LOGIN_SUCCESS':
        // user finished provider, now completing on our side
        this.setLoadingState(true, this.i18n.loadingCompleting);
        this.finishPopupFlow({ stopLoading: false, closePopup: true });

        of(msg.userData)
          .pipe(
            switchMap((user) => this.authService.externalLogin(user!)),
            catchError(() => {
              this.toastKey('error', this.i18n.authErrorSummary, this.i18n.authErrorDetail);
              return of(false);
            }),
            finalize(() => this.setLoadingState(false))
          )
          .subscribe((success) => {
            if (!success) {
              this.toastKey('error', this.i18n.loginFailedSummary, this.i18n.loginFailedDetail);
              this.setActions({ retry: true, openTab: !!this.lastFullUrl });
            } else {
              // on success, disable actions
              this.setActions({ retry: false, openTab: false });
            }
          });
        break;

      case 'EXTERNAL_LOGIN_ERROR':
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        this.toast(
          'error',
          this.i18nText(this.i18n.externalAuthFailedSummary),
          msg.content ?? msg.message ?? this.i18nText(this.i18n.externalAuthFailedDetailFallback)
        );
        this.setActions({ retry: true, openTab: !!this.lastFullUrl });
        break;

      case 'EXTERNAL_POPUP_CLOSED':
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
        this.setActions({ retry: true, openTab: !!this.lastFullUrl });
        break;

      default:
        break;
    }
  }

  /** Utilities */
  private centeredPosition() {
    const dualLeft = window.screenLeft ?? window.screenX ?? 0;
    const dualTop = window.screenTop ?? window.screenY ?? 0;
    const width = window.innerWidth || document.documentElement.clientWidth || screen.width;
    const height = window.innerHeight || document.documentElement.clientHeight || screen.height;
    const left = Math.max(0, dualLeft + (width - this.popupWidth) / 2);
    const top = Math.max(0, dualTop + (height - this.popupHeight) / 2);
    return { left, top };
  }

  private buildAllowedOrigins(): string[] {
    const origins = [window.location.origin];

    try {
      origins.push(new URL(environment.apiBaseUrl).origin);
    } catch {
      // Keep app origin fallback if API base URL is malformed.
    }

    return [...new Set(origins)];
  }

  private closePopup(): void {
    if (!this.popup) return;

    try {
      if (!this.safeIsPopupClosed()) {
        this.popup.close();
      }
    } catch {
      // Ignore COOP/Cross-Origin errors on close
    } finally {
      this.popup = null;
    }
  }

  private setLoadingState(isLoading: boolean, statusKey: string | null = null): void {
    if (isLoading && !this.externalLoadingActive) {
      this.loadingService.start();
      this.externalLoadingActive = true;
    }

    if (!isLoading && this.externalLoadingActive) {
      this.loadingService.stop();
      this.externalLoadingActive = false;
    }

    this.loading = isLoading;
    this.popupStatusKey = isLoading ? statusKey : null;
  }

  private setActions(opts: { retry: boolean; openTab: boolean }) {
    this.canRetry = opts.retry;
    this.canOpenInNewTab = opts.openTab;
  }

  private toastKey(
    severity: 'success' | 'info' | 'warn' | 'error',
    summaryKey: string,
    detailKey: string
  ) {
    switch (severity) {
      case 'success':
        this.notificationService.success(this.i18nText(detailKey), this.i18nText(summaryKey));
        break;
      case 'info':
        this.notificationService.info(this.i18nText(detailKey), this.i18nText(summaryKey));
        break;
      case 'warn':
        this.notificationService.warn(this.i18nText(detailKey), this.i18nText(summaryKey));
        break;
      case 'error':
        this.notificationService.error(this.i18nText(detailKey), this.i18nText(summaryKey));
        break;
    }
  }

  private toast(severity: 'success' | 'info' | 'warn' | 'error', summary: string, detail: string) {
    switch (severity) {
      case 'success':
        this.notificationService.success(detail, summary);
        break;
      case 'info':
        this.notificationService.info(detail, summary);
        break;
      case 'warn':
        this.notificationService.warn(detail, summary);
        break;
      case 'error':
        this.notificationService.error(detail, summary);
        break;
    }
  }

  private generateState(): string {
    // crypto-safe state
    try {
      const arr = new Uint8Array(16);
      crypto.getRandomValues(arr);
      return Array.from(arr, (b) => b.toString(16).padStart(2, '0')).join('');
    } catch {
      return Math.random().toString(36).slice(2);
    }
  }

  /** teardown */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.finishPopupFlow({ stopLoading: true, closePopup: true });
  }
}