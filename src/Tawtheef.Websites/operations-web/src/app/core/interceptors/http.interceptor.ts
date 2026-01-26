import { inject } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { environment } from '../../../environments/environment';
import { routes } from '../../routes/routes';

export const customHttpInterceptor: HttpInterceptorFn = (req, next) => {
  const modifiedRequest = addHeadersConditionally(req);
  logRequest(modifiedRequest);
  return executeRequest(modifiedRequest, next);
};

function executeRequest(request: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> {
  return next(request).pipe(
    tap({
      next: (event: HttpEvent<any>) => handleResponse(event, request)
    })
  );
}

/**
 * Add headers ONLY for your API (or same-origin).
 * Do NOT add custom headers for third-party domains to avoid CORS preflight failures.
 */
function addHeadersConditionally(request: HttpRequest<any>): HttpRequest<any> {
  const translate = inject(TranslateService);
  const currentLang = translate.getCurrentLang() || translate.getFallbackLang() || 'en';

  // Always safe to keep request untouched for external calls
  if (isExternalRequest(request.url)) {
    // If you still want language for external calls, you can keep Accept-Language only,
    // but safest is to add nothing.
    return request;
  }

  const headers: Record<string, string> = {
    'Accept-Language': currentLang,
    'X-Request-ID': generateRequestId(),
  };

  return request.clone({ setHeaders: headers });
}

function isExternalRequest(url: string): boolean {
  // Absolute URL => parse host; Relative URL => same origin
  if (!/^https?:\/\//i.test(url)) return false;

  const host = new URL(url).host.toLowerCase();

  // Blocklist the known geo-ip providers (and generally any 3rd party)
  const externalHosts = new Set([
    'ipapi.co',
    'ipwhois.app',
    'www.geoplugin.net'
  ]);

  if (externalHosts.has(host)) return true;

  // If you have a dedicated API base URL, only treat THAT as internal:
  // Example: environment.apiBaseUrl = 'https://my-api.domain.com/'
  const apiBase = environment.apiBaseUrl ?? '';
  if (apiBase && /^https?:\/\//i.test(apiBase)) {
    const apiHost = new URL(apiBase).host.toLowerCase();
    return host !== apiHost;
  }

  // Fallback: treat any absolute URL not matching current origin as external
  return host !== window.location.host.toLowerCase();
}

function generateRequestId(): string {
  return Math.random().toString(36).substring(2, 15) +
    Math.random().toString(36).substring(2, 15);
}

function handleResponse(event: HttpEvent<any>, request: HttpRequest<any>): void {
  if (event instanceof HttpResponse && event.body?.data !== undefined) {
    if (!environment.production) {
      console.log(`API Success: ${request.method} ${request.url}`);
    }

    const response = event as HttpResponse<any>;
    if (response.body && typeof response.body === 'object') {
      if (response.body.success === false) {
        throw new HttpErrorResponse({
          error: response.body.error || response.body,
          headers: response.headers,
          status: 400,
          statusText: 'Bad Request',
          url: response.url || undefined
        });
      } else if (response.body.data !== undefined) {
        (event as any).body = response.body.data;
      }
    }
  }
}

function logRequest(request: HttpRequest<any>): void {
  if (!environment.production && !request.url.includes(routes.auth.auth)) {
    console.log(`Making ${request.method} request to ${request.urlWithParams}`);
    if (request.body && !(request.body instanceof FormData)) {
      console.log('Request body:', request.body);
    }
  }
}
