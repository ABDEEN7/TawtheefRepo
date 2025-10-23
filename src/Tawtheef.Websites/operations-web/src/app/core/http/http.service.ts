import { HttpClient, HttpEvent, HttpParams, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { retryBackoff } from '../rxjs/retry-backoff';

/**
 * Generic HttpService wrapper around HttpClient that:
 * - supports retryBackoff when options.retry is true
 * - provides overloads so TS knows the correct Observable return type based on options.observe
 */
@Injectable({ providedIn: 'root' })
export class HttpService {
  constructor(private http: HttpClient) {}

  // ----------------------
  // GET
  // ----------------------
  public get<T>(url: string, params?: any, options?: { observe?: 'body' } & any): Observable<T>;
  public get<T>(url: string, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.get<T>(url, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  // ----------------------
  // POST
  // ----------------------
  public post<T>(url: string, body: any, params?: any, options?: { observe?: 'body' } & any): Observable<T>;
  public post<T>(url: string, body: any, params?: any, options?: { observe: 'response' } & any): Observable<HttpResponse<T>>;
  public post<T>(url: string, body: any, params?: any, options?: { observe: 'events' } & any): Observable<HttpEvent<T>>;
  public post<T>(url: string, body: any, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.post<T>(url, body, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  // ----------------------
  // PUT
  // ----------------------
  public put<T>(url: string, body: any, params?: any, options?: { observe?: 'body' } & any): Observable<T>;
  public put<T>(url: string, body: any, params?: any, options?: { observe: 'response' } & any): Observable<HttpResponse<T>>;
  public put<T>(url: string, body: any, params?: any, options?: { observe: 'events' } & any): Observable<HttpEvent<T>>;
  public put<T>(url: string, body: any, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.put<T>(url, body, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  // ----------------------
  // DELETE
  // ----------------------
  public delete<T>(url: string, params?: any, options?: { observe?: 'body' } & any): Observable<T>;
  public delete<T>(url: string, params?: any, options?: { observe: 'response' } & any): Observable<HttpResponse<T>>;
  public delete<T>(url: string, params?: any, options?: { observe: 'events' } & any): Observable<HttpEvent<T>>;
  public delete<T>(url: string, params?: any, options: any = {}): Observable<T | HttpEvent<T>> {
    const request$ = this.http.delete<T>(url, { params: params, ...options });
    if (options.retry) return request$.pipe(retryBackoff(options.retryCount || 3, options.retryDelay || 500));
    return request$;
  }

  // ----------------------
  // REQUEST (generic HttpRequest)
  // ----------------------
  public request<T>(req: HttpRequest<any>, params?: any, options?: { observe?: 'body' } & any): Observable<T>;
  public request<T>(req: HttpRequest<any>, params?: any, options?: { observe: 'response' } & any): Observable<HttpResponse<T>>;
  public request<T>(req: HttpRequest<any>, params?: any, options?: { observe: 'events' } & any): Observable<HttpEvent<T>>;

  // Support calling like request<T>(method: string, url: string, options?)
  public request<T>(method: string, url: string, options?: { observe?: 'body' } & any): Observable<T>;
  public request<T>(method: string, url: string, options?: { observe: 'response' } & any): Observable<HttpResponse<T>>;
  public request<T>(method: string, url: string, options?: { observe: 'events' } & any): Observable<HttpEvent<T>>;

  // Implementation: unify handling
  public request<T>(
    arg1: HttpRequest<any> | string,
    arg2?: any,
    arg3: any = {}
  ): Observable<T | HttpEvent<T>> {
    let request$!: Observable<T | HttpEvent<T>>;

    // --- Case 1: caller passed HttpRequest instance ---
    if (arg1 instanceof HttpRequest) {
      const req = arg1 as HttpRequest<any>;
      const paramsObject = arg2;
      const options = arg3 || {};

      // Build new HttpParams if provided as plain object
      const mergedParams =
        paramsObject instanceof HttpParams
          ? paramsObject
          : paramsObject
            ? new HttpParams({ fromObject: paramsObject })
            : req.params;

      // Clone request with new params and options
      const cloned = req.clone({
        params: mergedParams,
        reportProgress: options.reportProgress ?? req.reportProgress,
        withCredentials: options.withCredentials ?? req.withCredentials,
        responseType: options.responseType ?? (req.responseType as any),
      });

      request$ = this.http.request<T>(cloned);

      if (options.retry)
        return request$.pipe(
          retryBackoff(options.retryCount || 3, options.retryDelay || 500)
        );

      return request$;
    }

    // --- Case 2: caller used (method, url, options) style ---
    const method = arg1 as string;
    const url = arg2 as string;
    const options = arg3 || {};

    request$ = this.http.request<T>(method, url, { ...options });

    if (options.retry)
      return request$.pipe(
        retryBackoff(options.retryCount || 3, options.retryDelay || 500)
      );

    return request$;
  }
}
