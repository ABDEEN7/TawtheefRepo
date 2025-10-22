// src/app/interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, from } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import {AuthService} from "../auth/auth.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.auth.getAccessToken();

    let authReq = req;
    if (accessToken) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      });
    }

    return next.handle(authReq).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          // try to refresh token then retry request
          return from(this.auth.refreshToken()).pipe(
            switchMap(newTokens => {
              if (newTokens && newTokens.accessToken) {
                const cloned = req.clone({
                  setHeaders: {
                    Authorization: `Bearer ${newTokens.accessToken}`
                  }
                });
                return next.handle(cloned);
              }
              // no tokens -> propagate original error
              return throwError(() => err);
            })
          );
        }
        return throwError(() => err);
      })
    );
  }
}
