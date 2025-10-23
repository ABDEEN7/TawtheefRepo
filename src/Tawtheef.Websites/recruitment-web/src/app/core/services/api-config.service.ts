import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiConfigService {
  private _baseUrl: string = environment.apiBaseUrl;

  get baseUrl(): string {
    return this._baseUrl;
  }
}
