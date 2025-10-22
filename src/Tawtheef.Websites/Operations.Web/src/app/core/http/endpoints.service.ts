import { Injectable } from '@angular/core';
import {environment} from "../../../environments/environment";

@Injectable({ providedIn: 'root' })
export class EndpointsService {
  constructor() {}

  get base() { return environment.apiBaseUrl; }

  auth = {
    login: () => `${this.base}/auth/login`,
    refresh: () => `${this.base}/auth/refresh`,
    externalLogin: (provider: string) => `${this.base}/auth/external/${provider}`,
    logout: ()=> `${this.base}/auth/logout`,
    exchangeCode: () => `${this.base}/auth/exchange-code`
  };

  files = {
    upload: () => `${this.base}/files/upload`,
    download: (id: string) => `${this.base}/files/${id}/download`
  };

  // add other grouped endpoints here
}
