import {inject, Injectable, NgZone, OnDestroy} from '@angular/core';
import {AuthService} from '../../core/auth/auth.service';
import {MessageService} from 'primeng/api';
import {EndpointsService} from '../../core/http/endpoints.service';
import {TranslateService} from '@ngx-translate/core';
import {LoadingService} from '../../core/services/loading.service';
import {msalInstance} from '../../core/config/msal-config';
import {AuthenticationResult, PopupRequest} from '@azure/msal-browser';
import {environment} from '../../../environments/environment';
import {AuthResponse} from './login/models/auth-response.model';
import {HttpClient} from '@angular/common/http';

@Injectable({providedIn: 'root'})
export class ExternalLoginService implements OnDestroy {
  private readonly popupWidth: number = 600;
  private readonly popupHeight: number = 800;
  private ngZone: NgZone = inject(NgZone);
  private popup: Window | null = null;
  private popupCloseListener: any;

  loading = false;
  protected constructor(loadingService: LoadingService, public authService: AuthService,
                        public messageService: MessageService, public endpoints: EndpointsService,
                        public translate: TranslateService, public http: HttpClient) {
    loadingService.loading$.subscribe(loading => {
      this.loading = loading;
    });
    window.addEventListener('message', this.handlePopupMessage.bind(this), false);
  }

  public signInWithGoogle(): void {
    const url = this.endpoints.auth.externalLogin('Google');

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
  public signInWithAzure(): void {
    if (this.popup) {
      this.popup.close();
      this.popup = null;
    }
    const azureLoginRequest: PopupRequest = {
      scopes: ["openid", "profile", "email"]
    };
    msalInstance.loginPopup(azureLoginRequest)
      .then((result: AuthenticationResult) => {
        const idToken = result.idToken;
        const data = {
          provider: 'AzureAD',
          idToken, // send id token to backend
          account: result.account ? {
            username: result.account.username,
            homeAccountId: result.account.homeAccountId,
            name: (result.account.idTokenClaims && (result.account.idTokenClaims as any).name) || ''
          } : null
        };
        this.ngZone.run(() => {
          this.http.post<AuthResponse>(this.endpoints.auth.externalLoginUsingToken, data).subscribe((user)=>{
            this.authService.externalLogin(user).subscribe({
              next: (success) => {
                if (!success) {
                  this.messageService.add({
                    severity: 'error',
                    summary: 'Login Failed',
                    detail: 'Could not authenticate with Azure AD'
                  });
                }
              },
              error: (err) => {
                console.error('External login error', err);
                this.messageService.add({
                  severity: 'error',
                  summary: 'Error',
                  detail: 'An error occurred during authentication'
                });
              }
            });
          })
        });
      })
      .catch((error) => {
        console.error('Azure login popup error', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Login Failed',
          detail: (error && error.errorMessage) ? error.errorMessage : 'Azure authentication failed'
        });
      });
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
