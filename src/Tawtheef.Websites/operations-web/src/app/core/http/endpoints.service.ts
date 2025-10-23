import { Injectable } from '@angular/core';
import {environment} from "../../../environments/environment";

@Injectable({ providedIn: 'root' })
export class EndpointsService {
  constructor() {}

  get base() { return environment.apiBaseUrl; }

  auth = {
    me: `${this.base}/auth/me`,
    login: `${this.base}/auth/login`,
    register: `${this.base}/auth/register`,
    refresh: `${this.base}/auth/refresh`,
    externalLogin: (provider: string) => `${this.base}/auth/external/${provider}`,
    logout: `${this.base}/auth/logout`,
    exchangeCode: `${this.base}/auth/exchange-code`,
    verifyOtp: `${this.base}/auth/verify-otp`,
    forgotPassword: `${this.base}/auth/forgot-password`,
    resetPassword: `${this.base}/auth/reset-password`,
    resendOtp: `${this.base}/auth/resend-otp`,
  };

  user= {
    profile: {
      socialAccounts: `${this.base}/user/profile/social-accounts`,
    }
  };

  files = {
    upload: `${this.base}/files/upload`,
    download: (id: string) => `${this.base}/files/${id}/download`
  };

  // add other grouped endpoints here
}
