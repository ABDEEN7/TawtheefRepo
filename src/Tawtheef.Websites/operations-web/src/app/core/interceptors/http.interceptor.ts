import {inject} from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
  HttpResponse
} from '@angular/common/http';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import {TranslateService} from "@ngx-translate/core";
import {environment} from "../../../environments/environment";
import {routes} from '../../routes/routes';
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
  if (event instanceof HttpResponse) {
    const body: any = event.body;
    const hasData = body?.data !== undefined || body?.Data !== undefined;
    const payload = body?.data ?? body?.Data;
    const isFailure = body?.success === false || body?.Success === false;

    if (!environment.production) {
      console.log(`API Success: ${request.method} ${request.url}`);
    }

    if (body && typeof body === 'object') {
      if (isFailure) {
        throw new HttpErrorResponse({
          error: body.error || body.Error || body,
          headers: event.headers,
          status: 400,
          statusText: 'Bad Request',
          url: event.url || undefined
        });
      } else if (hasData) {
        (event as any).body = payload;
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
