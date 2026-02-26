import { inject, Injectable, NgZone, OnDestroy } from '@angular/core';
import { AuthService } from './auth.service';
import { EndpointsService } from '../http/endpoints.service';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../../environments/environment';
import { fromEvent, interval, merge, Subject, Subscription, timer } from 'rxjs';
import { filter, finalize, map, takeUntil } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';
import { TranslateService } from '@ngx-translate/core';
import { OAUTH_STATE_KEY } from '../constants/auth-tokens.const';

type ExternalMessageType = 'EXTERNAL_LOGIN_SUCCESS' | 'EXTERNAL_LOGIN_ERROR' | 'EXTERNAL_POPUP_CLOSED';
interface ExternalMessage {
  type: ExternalMessageType;
  userData?: any;
  message?: { message: string }[];
}

@Injectable({ providedIn: 'root' })
export class ExternalLoginService implements OnDestroy {
  private readonly destroy$ = new Subject<void>();
  private popup: Window | null = null;
  private popupDone$: Subject<void> | null = null;
  private popupPollSub: Subscription | null = null;
  private popupTimeoutSub: Subscription | null = null;
  private popupFocusSub: Subscription | null = null;

  private readonly popupWidth = 600;
  private readonly popupHeight = 800;
  private readonly popupTimeoutMs = 60_000;

  private readonly ngZone = inject(NgZone);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly endpoints = inject(EndpointsService);
  private readonly loadingService = inject(LoadingService);
  private readonly translate = inject(TranslateService);

  loading = false;
  popupStatusKey: string | null = null;

  private externalLoadingActive = false;
  private readonly allowedOrigins = this.buildAllowedOrigins();

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

    popupClosedSummary: 'auth.externalLogin.popupClosed.summary',
    popupClosedDetail: 'auth.externalLogin.popupClosed.detail',
    timeoutSummary: 'auth.externalLogin.timeout.summary',
    timeoutDetail: 'auth.externalLogin.timeout.detail',

    loadingOpening: 'auth.externalLogin.loadingOpening',
    loadingCompleting: 'auth.externalLogin.loadingCompleting',
  } as const;

  constructor() {
    fromEvent<MessageEvent>(window, 'message')
      .pipe(
        takeUntil(this.destroy$),
        filter((event) => this.isAllowedOrigin(event.origin)),
        map((event) => event.data as ExternalMessage)
      )
      .subscribe((msg) => this.ngZone.run(() => this.handleMessage(msg)));
  }

  private i18nText(key: string, prefix = ''): string {
    const errorMessage = [prefix, key].filter((segment) => !!segment).join('.');
    const translated = this.translate.instant(errorMessage);
    if (translated === errorMessage) {
      return key;
    }

    return translated;
  }

  private safeIsPopupClosed(): boolean {
    if (!this.popup) return true;

    try {
      // This is the property that triggers the COOP error
      return this.popup.closed;
    } catch (e) {
      // If we hit a security error, it means the popup is
      // definitely still open but on a different domain (Google).
      // We return false so the timer keeps running.
      return false;
    }
  }

  /** Starts the Azure login process */
  loginUsingAzure(): void {
    const url = this.endpoints.auth.externalLogin('Azure');
    this.startNewPopupFlow(url, this.i18n.popupBlockedDetailShort, this.centerPopup());
  }

  public loginUsingGoogle(): void {
    const url = this.endpoints.auth.externalLogin('google');
    this.openPopupWithState(url);
  }

  private openPopupWithState(baseUrl: string): void {
    const state = Math.random().toString(36).slice(2);
    localStorage.setItem(OAUTH_STATE_KEY, state);
    const url = baseUrl.includes('?') ? `${baseUrl}&state=${state}` : `${baseUrl}?state=${state}`;

    this.startNewPopupFlow(url, this.i18n.popupBlockedDetailContinue, this.centeredPosition());
  }

  private startNewPopupFlow(
    url: string,
    popupBlockedDetailKey: string,
    position: { left: number; top: number }
  ): void {
    this.finishPopupFlow({ stopLoading: true, closePopup: true });
    this.popupDone$ = new Subject<void>();

    this.setLoadingState(true, this.i18n.loadingOpening);
    this.popup = window.open(
      url,
      '_external_login',
      `width=${this.popupWidth},height=${this.popupHeight},left=${position.left},top=${position.top},resizable=yes,scrollbars=yes`
    );

    if (this.safeIsPopupClosed()) {
      this.finishPopupFlow({ stopLoading: true, closePopup: true });
      this.toastKey('warn', this.i18n.popupBlockedSummary, popupBlockedDetailKey);
      return;
    }

    this.setLoadingState(true, this.i18n.loadingCompleting);

    const untilPopupDone$ = merge(this.destroy$, this.popupDone$);

    this.popupPollSub = interval(350)
      .pipe(takeUntil(untilPopupDone$))
      .subscribe(() => {
        if (this.safeIsPopupClosed()) {
          this.finishPopupFlow({ stopLoading: true, closePopup: true });
          this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
        }
      });

    this.popupTimeoutSub = timer(this.popupTimeoutMs)
      .pipe(takeUntil(untilPopupDone$))
      .subscribe(() => {
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        this.toastKey('warn', this.i18n.timeoutSummary, this.i18n.timeoutDetail);
      });

    this.popupFocusSub = fromEvent(window, 'focus')
      .pipe(takeUntil(untilPopupDone$))
      .subscribe(() => {
        if (this.popup && this.safeIsPopupClosed()) {
          this.finishPopupFlow({ stopLoading: true, closePopup: true });
          this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
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

  /** Handles all incoming messages */
  private handleMessage(msg: ExternalMessage): void {
    switch (msg.type) {
      case 'EXTERNAL_LOGIN_SUCCESS':
        this.setLoadingState(true, this.i18n.loadingCompleting);
        this.finishPopupFlow({ stopLoading: false, closePopup: true });
        this.authService
          .externalLogin(msg.userData)
          .pipe(finalize(() => this.setLoadingState(false)))
          .subscribe({
            next: (success) =>
              success
                ? this.toastKey('success', this.i18n.loginSuccessfulSummary, this.i18n.loginSuccessfulDetail)
                : this.toastKey('error', this.i18n.loginFailedSummary, this.i18n.loginFailedDetailProvider),
            error: () =>
              this.toastKey('error', this.i18n.authErrorSummary, this.i18n.authErrorDetailLogin),
          });
        break;

      case 'EXTERNAL_LOGIN_ERROR': {
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        const errorList = msg.message;
        this.toast(
          'error',
          this.i18nText(this.i18n.loginFailedSummary),
          errorList?.map((item) => this.i18nText(item.message, 'server-error')).join('\n')
            ?? this.i18nText(this.i18n.externalAuthFailedDetailFallback),
        );
        break;
      }

      case 'EXTERNAL_POPUP_CLOSED':
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
        break;
    }
  }

  /** Checks allowed origins */
  private isAllowedOrigin(origin: string): boolean {
    return this.allowedOrigins.includes(origin);
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

  /** Closes popup safely */
  private closePopup(): void {
    if (!this.safeIsPopupClosed()) {
      this.popup!.close();
    }
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

  /** Utilities */
  private centeredPosition() {
    // robust centering across multi-monitor setups
    const dualLeft = window.screenLeft ?? window.screenX ?? 0;
    const dualTop = window.screenTop ?? window.screenY ?? 0;
    const width = window.innerWidth || document.documentElement.clientWidth || screen.width;
    const height = window.innerHeight || document.documentElement.clientHeight || screen.height;
    const left = Math.max(0, dualLeft + (width - this.popupWidth) / 2);
    const top = Math.max(0, dualTop + (height - this.popupHeight) / 2);
    return { left, top };
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

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.finishPopupFlow({ stopLoading: true, closePopup: true });
  }
}
