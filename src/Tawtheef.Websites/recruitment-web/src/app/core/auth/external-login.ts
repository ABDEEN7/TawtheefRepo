import {inject, Injectable, NgZone, OnDestroy} from '@angular/core';
import {AuthService} from './auth.service';
import {EndpointsService} from '../http/endpoints.service';
import {LoadingService} from '../services/loading.service';
import {environment} from '../../../environments/environment';
import {fromEvent, interval, of, Subject} from 'rxjs';
import {catchError, filter, map, switchMap, takeUntil, tap} from 'rxjs/operators';
import {ExternalMsg} from '../../pages/auth/popup-callback/popup-callback';
import {TranslateService} from '@ngx-translate/core';
import {NotificationService} from '../services/notification.service';
import {OAUTH_STATE_KEY} from '../constants/auth-tokens.const';

@Injectable({ providedIn: 'root' })
export class ExternalLoginService implements OnDestroy {
  private readonly destroy$ = new Subject<void>();

  private readonly popupWidth = 600;
  private readonly popupHeight = 800;
  private popup: Window | null = null;

  private readonly ngZone = inject(NgZone);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly endpoints = inject(EndpointsService);
  private readonly loadingService = inject(LoadingService);
  private readonly translate = inject(TranslateService);

  loading = false;

  // allow-list the origins that are permitted to postMessage back
  private readonly allowedOrigins = new Set<string>([
    window.location.origin,
    environment.apiBaseUrl
  ]);

  // i18n keys (service-local)
  private readonly i18n = {
    popupBlockedSummary: 'auth.externalLogin.popupBlocked.summary',
    popupBlockedDetail: 'auth.externalLogin.popupBlocked.detail',

    authErrorSummary: 'auth.externalLogin.authError.summary',
    authErrorDetail: 'auth.externalLogin.authError.detail',

    loginFailedSummary: 'auth.externalLogin.loginFailed.summary',
    loginFailedDetail: 'auth.externalLogin.loginFailed.detail',

    externalAuthFailedSummary: 'auth.externalLogin.externalAuthFailed.summary',
    externalAuthFailedDetailFallback: 'auth.externalLogin.externalAuthFailed.detailFallback'
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

  loginAsQatarResident(){
    // open popup
    // bass success auth
  }

  /** Open popup (centered), add `state`, and start polling for manual close */
  private openPopupWithState(baseUrl: string): void {
    this.closePopup();

    const state = Math.random().toString(36).slice(2);
    localStorage.setItem(OAUTH_STATE_KEY, state);
    const url = baseUrl.includes('?') ? `${baseUrl}&state=${state}` : `${baseUrl}?state=${state}`;

    const { left, top } = this.centeredPosition();
    this.popup = window.open(
      url,
      '_external_login',
      `width=${this.popupWidth},height=${this.popupHeight},left=${left},top=${top},resizable=yes,scrollbars=yes`
    );

    if (this.safeIsPopupClosed()) {
      this.toastKey('warn', this.i18n.popupBlockedSummary, this.i18n.popupBlockedDetail);
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
              this.toastKey('error', this.i18n.authErrorSummary, this.i18n.authErrorDetail);
              return of(false);
            })
          )
          .subscribe(success => {
            if (!success) {
              this.toastKey('error', this.i18n.loginFailedSummary, this.i18n.loginFailedDetail);
            }
          });
        this.closePopup();
        break;

      case 'EXTERNAL_LOGIN_ERROR':
        // If msg.content is already localized by the popup, you can pass it through.
        // Otherwise, use a fallback key.
        this.toast(
          'error',
          this.i18nText(this.i18n.externalAuthFailedSummary),
          msg.content ?? this.i18nText(this.i18n.externalAuthFailedDetailFallback)
        );
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

  /** teardown */
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.closePopup();
  }
}
