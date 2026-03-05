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

  loading = false;
  popupStatusKey: string | null = null;

  private externalLoadingActive = false;
  private readonly allowedOrigins = this.buildAllowedOrigins();

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
    loadingCompleting: 'auth.externalLogin.loadingCompleting',
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
    const url = this.endpoints.auth.externalLogin('google');
    this.openPopupWithState(url);
  }

  public loginUsingQatarPass(): void {
    const url = environment.qatarPassLoginUrl;
    this.openPopupWithState(url);
  }

  loginAsQatarResident() {
    // open popup
    // bass success auth
  }

  private i18nText(key: string): string {
    return this.translate.instant(key);
  }

  private safeIsPopupClosed(): boolean {
    if (!this.popup) return true;

    try {
      // `closed` غالبًا مسموح حتى مع cross-origin
      return this.popup.closed;
    } catch {
      // لا تعمل "block" نهائي. خليه يحاول مرة ثانية بالـ poll القادم.
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
    const state = Math.random().toString(36).slice(2);
    localStorage.setItem(OAUTH_STATE_KEY, state);
    const url = baseUrl.includes('?') ? `${baseUrl}&state=${state}` : `${baseUrl}?state=${state}`;

    this.startNewPopupFlow(url, this.centeredPosition());
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

    if (this.safeIsPopupClosed()) {
      this.finishPopupFlow({ stopLoading: true, closePopup: true });
      this.toastKey('warn', this.i18n.popupBlockedSummary, this.i18n.popupBlockedDetail);
      return;
    }

    this.setLoadingState(true, this.i18n.loadingCompleting);

    const untilPopupDone$ = merge(this.destroy$, this.popupDone$);

    this.isAccessRestricted = this.isCrossChain(url);

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

  /** Handle messages from popup */
  private handleMessage(msg: ExternalMsg): void {
    switch (msg.type) {
      case 'EXTERNAL_LOGIN_SUCCESS':
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
        break;

      case 'EXTERNAL_POPUP_CLOSED':
        this.finishPopupFlow({ stopLoading: true, closePopup: true });
        this.toastKey('warn', this.i18n.popupClosedSummary, this.i18n.popupClosedDetail);
        break;

      default:
        // ignore unknown messages
        break;
    }
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

  /** teardown */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.finishPopupFlow({ stopLoading: true, closePopup: true });
  }
}
