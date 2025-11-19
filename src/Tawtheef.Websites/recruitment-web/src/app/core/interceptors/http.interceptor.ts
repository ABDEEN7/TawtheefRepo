import {inject, Injectable} from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler, HttpHandlerFn,
  HttpInterceptor, HttpInterceptorFn,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {TranslateService} from "@ngx-translate/core";
import {environment} from "../../../environments/environment";
import {routes} from '../../routes/routes';
import {TokenService} from '../auth/token.service';
export const customHttpInterceptor: HttpInterceptorFn = (req, next) => {
  const modifiedRequest = addHeaders(req);
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

function addHeaders(request: HttpRequest<any>): HttpRequest<any> {
  const translate = inject(TranslateService);
  const currentLang = translate.getCurrentLang() || translate.getFallbackLang();
  const headers: { [key: string]: string } = {
    'Accept-Language': currentLang!,
    'X-Request-ID': generateRequestId(),
  };
  return request.clone({ setHeaders: headers });
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

    // Modify the response body to return just the data property if it exists
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
        // If there's a data property, return just that
        (event as any).body = response.body.data;
      }
      // else keep the original body
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
