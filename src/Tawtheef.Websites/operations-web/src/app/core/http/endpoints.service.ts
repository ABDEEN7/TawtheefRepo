import {inject, Injectable} from '@angular/core';
import {ApiConfigService} from '../services/api-config.service';
import {CaseUtils} from '../utils/case-utils';

@Injectable({ providedIn: 'root' })
export class EndpointsService {
  apiConfig = inject(ApiConfigService);

  private getFullUrl(endpoint: string): string {
    return `${this.apiConfig.baseUrl}${endpoint}`;
  }

  logger = this.getFullUrl(`/logger`);
  auth = {
    me: this.getFullUrl(this.getFullUrl(`/auth/me`)),
    login: this.getFullUrl(`/auth/login`),
    register: this.getFullUrl(`/auth/register`),
    refresh: this.getFullUrl(`/auth/refresh-token`),
    externalLogin: (provider: string, returnUrl: string | null = null) =>
      this.getFullUrl(`/auth/external-login?provider=${CaseUtils.toPascalCase(provider)}${returnUrl ? `&returnUrl=${encodeURIComponent(returnUrl)}` : ''}`),
    externalLoginUsingToken: this.getFullUrl(`/auth/external-login/token`),
    logout: this.getFullUrl(`/auth/logout`),
    exchangeCode: this.getFullUrl(`/auth/exchange-code`),
    verifyOtp: this.getFullUrl(`/auth/verify-otp`),
    forgotPassword: this.getFullUrl(`/auth/forgot-password`),
    resetPassword: this.getFullUrl(`/auth/reset-password`),
    resendOtp: this.getFullUrl(`/auth/resend-otp`),
  };

  user= {
    profile: {
      socialAccounts: this.getFullUrl(`/user/profile/social-accounts`),
    }
  };

  files = {
    upload: this.getFullUrl(`/files/upload`),
    download: (id: string) => this.getFullUrl(`/files/${id}/download`)
  };
}
