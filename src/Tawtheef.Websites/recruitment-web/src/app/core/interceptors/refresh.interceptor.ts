import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, Subject, throwError } from 'rxjs';
import { catchError, first, switchMap } from 'rxjs/operators';
import { TokenService } from '../auth/token.service';
import { AuthService } from '../auth/auth.service';
import { HDR } from '../utils/headers.flags';

let refreshInFlight = false;
const refreshDone$ = new Subject<boolean>();

export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenSvc = inject(TokenService);
  const authSvc  = inject(AuthService);

  return next(req).pipe(
    catchError((err: unknown) => {
      const httpErr = err as HttpErrorResponse;

      const is401 = httpErr.status === 401;
      const isAuthCall = /\/auth\/(login|refresh|external)/i.test(req.url);
      const skipRefresh = req.headers.get(HDR.SkipRefresh) === 'true';
      const alreadyRetried = req.headers.get(HDR.Retried) === '1';

      // Not our job → pass along
      if (!is401 || isAuthCall || skipRefresh || alreadyRetried) {
        return throwError(() => err);
      }

      // If a refresh is already happening, wait for it and then retry once
      if (refreshInFlight) {
        return refreshDone$.pipe(
          first(),
          switchMap((ok) => ok
            ? next(attachLatestToken(markRetried(req), tokenSvc))
            : throwError(() => err))
        );
      }

      // Start a single refresh
      refreshInFlight = true;
      return from(authSvc.refreshToken()).pipe(
        switchMap(() => {
          refreshInFlight = false;
          refreshDone$.next(true);
          return next(attachLatestToken(markRetried(req), tokenSvc));
        }),
        catchError((refreshErr) => {
          refreshInFlight = false;
          refreshDone$.next(false);

          // IMPORTANT: local-only logout to avoid calling logout endpoint again
          authSvc.logout(false); // clear client & navigate; no API call
          return throwError(() => refreshErr);
        })
      );
    })
  );
};

function markRetried(req: any) {
  return req.clone({ setHeaders: { [HDR.Retried]: '1' } });
}
function attachLatestToken(req: any, tokenSvc: TokenService) {
  const t = tokenSvc.getToken();
  return t ? req.clone({ setHeaders: { Authorization: `Bearer ${t}` } }) : req;
}
