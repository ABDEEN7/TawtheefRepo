import {HttpClient, HttpEvent, HttpParams} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {retryBackoff} from "../rxjs/retry-backoff";

@Injectable({ providedIn: 'root' })
export class HttpService {
  constructor(private http: HttpClient) {}

  protected get<T>(url: string, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.get<T>(url, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  protected post<T>(url: string, body: any, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.post<T>(url, body, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  protected put<T>(url: string, body: any, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.put<T>(url, body, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  protected delete<T>(url: string, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.delete<T>(url, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }
}
