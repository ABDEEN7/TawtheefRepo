import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject, NgZone } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MessageService } from 'primeng/api';
import { AuthService } from '../auth/auth.service';
import { HDR } from '../utils/headers.flags';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.headers.get(HDR.SkipError) === 'true') return next(req);

  const msg  = inject(MessageService);
  const zone = inject(NgZone);
  // const auth = inject(AuthService); // if you want 401 toast

  return next(req).pipe(
    catchError((err: unknown) => {
      if (!(err instanceof HttpErrorResponse)) return throwError(() => err);

      // Don't toast for auth infrastructure calls or silent flows
      const isInfraAuth = /\/auth\/(login|refresh|logout|external)/i.test(req.url);
      if (isInfraAuth || req.headers.get(HDR.LogoutFlow) === 'true') {
        return throwError(() => err);
      }

      if (err.status === 0) {
        zone.run(() => msg.add({ severity: 'error', summary: 'Network error', detail: 'Please check your connection and try again.', life: 6000 }));
        return throwError(() => err);
      }

      // (optional) other status handling...
      return throwError(() => err);
    })
  );
};
