import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TokenService } from '../auth/token.service';
import { HDR } from '../utils/headers.flags';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.headers.get(HDR.SkipAuth) === 'true') return next(req);

  const token = inject(TokenService).getToken();
  const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
  return next(authReq);
};
