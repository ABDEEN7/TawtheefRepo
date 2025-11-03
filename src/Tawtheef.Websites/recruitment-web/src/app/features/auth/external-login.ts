import {inject, Injectable, NgZone, OnDestroy} from '@angular/core';
import {AuthService} from '../../core/auth/auth.service';
import {MessageService} from 'primeng/api';
import {EndpointsService} from '../../core/http/endpoints.service';
import {LoadingService} from '../../core/services/loading.service';
import {environment} from '../../../environments/environment';

@Injectable({providedIn: 'root'})
export class ExternalLoginService implements OnDestroy {
  private readonly popupWidth: number = 600;
  private readonly popupHeight: number = 800;
  private ngZone: NgZone = inject(NgZone);
  private popup: Window | null = null;
  private popupCloseListener: any;

  loading = false;
  protected constructor(loadingService: LoadingService, private authService: AuthService,
                        private messageService: MessageService, private endpoints: EndpointsService) {
    loadingService.loading$.subscribe(loading => {
      this.loading = loading;
    });
    window.addEventListener('message', this.handlePopupMessage.bind(this), false);
  }

  public loginUsingGoogle(provider: string): void {
    const url = this.endpoints.auth.externalLogin(provider);

    // Close any existing popup
    if (this.popup) {
      this.popup.close();
      this.popup = null;
    }

    // Calculate centered position
    const left = (window.screen.width - this.popupWidth) / 2;
    const top = (window.screen.height - this.popupHeight) / 2;

    // Add state parameter for security
    const state = Math.random().toString(36).substring(2);
    localStorage.setItem('oauth_state', state);
    const urlWithState = `${url}&state=${state}`;

    // Open popup
    this.popup = window.open(
      urlWithState,
      '_blank',
      `width=${this.popupWidth},height=${this.popupHeight},top=${top},left=${left}`
    );

    // Check if popup was blocked
    if (!this.popup || this.popup.closed || typeof this.popup.closed === 'undefined') {
      this.messageService.add({
        severity: 'warn',
        summary: 'Popup Blocked',
        detail: 'Please allow popups for this site to continue with external login'
      });
      return;
    }

    // Use content-based approach to detect popup closing
    this.popupCloseListener = (event: MessageEvent) => {
      if (event.data === 'EXTERNAL_POPUP_CLOSED') {
        this.cleanupPopup();
      }
    };
    window.addEventListener('message', this.popupCloseListener);
  }
  public loginUsingQatarPass(): void {
    const url = environment.qatarPassLoginUrl;

    // Close any existing popup
    if (this.popup) {
      this.popup.close();
      this.popup = null;
    }

    // Calculate centered position
    const left = (window.screen.width - this.popupWidth) / 2;
    const top = (window.screen.height - this.popupHeight) / 2;

    // Add state parameter for security
    const state = Math.random().toString(36).substring(2);
    localStorage.setItem('oauth_state', state);
    const urlWithState = `${url}&state=${state}`;

    // Open popup
    this.popup = window.open(
      urlWithState,
      'ExternalLogin',
      `width=${this.popupWidth},height=${this.popupHeight},top=${top},left=${left}`
    );

    // Check if popup was blocked
    if (!this.popup || this.popup.closed || typeof this.popup.closed === 'undefined') {
      this.messageService.add({
        severity: 'warn',
        summary: 'Popup Blocked',
        detail: 'Please allow popups for this site to continue with external login'
      });
      return;
    }

    // Use content-based approach to detect popup closing
    this.popupCloseListener = (event: MessageEvent) => {
      if (event.data === 'EXTERNAL_POPUP_CLOSED') {
        this.cleanupPopup();
      }
    };
    window.addEventListener('message', this.popupCloseListener);
  }
  private cleanupPopup(): void {
    if (this.popupCloseListener) {
      window.removeEventListener('message', this.popupCloseListener);
      this.popupCloseListener = null;
    }
    if (this.popup) {
      this.popup.close();
      this.popup = null;
    }
  }
  private handlePopupMessage(event: MessageEvent): void {
    // Ensure the content is from our domain
    const allowedOrigins = [window.location.origin, environment.apiBaseUrl]; // example
    if (!allowedOrigins.includes(event.origin)) return;

    // Handle the content data
    if (event.data.type === 'EXTERNAL_LOGIN_SUCCESS') {
      this.ngZone.run(() => {
        const userData = event.data.userData;
        this.authService.externalLogin(userData).subscribe({
          next: (success) => {
            if (!success) {
              this.messageService.add({
                severity: 'error',
                summary: 'Login Failed',
                detail: 'Could not authenticate with external provider'
              });
            }
          },
          error: () => {
            const message = event.data.content;
            console.error(message);
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'An error occurred during authentication'
            });
          }
        });
      });

      this.cleanupPopup();
    } else if (event.data.type === 'EXTERNAL_LOGIN_ERROR') {
      this.ngZone.run(() => {
        this.messageService.add({
          severity: 'error',
          summary: 'Login Failed',
          detail: event.data.content || 'External authentication failed'
        });
      });
      this.cleanupPopup();
    }
  }
  ngOnDestroy(): void {
    // Clean up
    window.removeEventListener('message', this.handlePopupMessage.bind(this));
    if (this.popupCloseListener) {
      window.removeEventListener('message', this.popupCloseListener);
    }
    this.cleanupPopup();
  }
}
