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

      // 1) Refresh only (logout only if THIS fails)
      return from(authSvc.refreshToken()).pipe(
        catchError((refreshErr) => {
          refreshInFlight = false;
          refreshDone$.next(false);

          authSvc.logout(false);
          return throwError(() => refreshErr);
        }),

        // 2) Retry original request (do NOT logout if this fails)
        switchMap(() => {
          refreshInFlight = false;
          refreshDone$.next(true);

          const retried = attachLatestToken(markRetried(req), tokenSvc);
          return next(retried); // if 500 happens here, it will bubble up normally
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
