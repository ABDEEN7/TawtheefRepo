import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { TokenService } from '../auth/token.service';
import { AuthStateService } from '../auth/auth-state.service';
import { HDR } from '../utils/headers.flags';

export const refreshInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenSvc = inject(TokenService);
  const authState = inject(AuthStateService);

  return next(req).pipe(
    catchError((err: unknown) => {
      const httpErr = err as HttpErrorResponse;

      const is401 = httpErr.status === 401;
      const isRefreshCall = /\/auth\/refresh-token$/i.test(req.url);
      const skipRefresh = req.headers.get(HDR.SkipRefresh) === 'true';
      const alreadyRetried = req.headers.get(HDR.Retried) === '1';

      if (!is401 || isRefreshCall || skipRefresh || alreadyRetried) {
        return throwError(() => err);
      }

      return authState.refreshAccessToken$().pipe(
        switchMap((ok) => {
          if (!ok) {
            authState.logout(false);
            return throwError(() => err);
          }

          const retried = attachLatestToken(markRetried(req), tokenSvc);
          return next(retried);
        }),
      );
    }),
  );
};

function markRetried(req: any) {
  return req.clone({ setHeaders: { [HDR.Retried]: '1' } });
}

function attachLatestToken(req: any, tokenSvc: TokenService) {
  const token = tokenSvc.getToken();
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}
