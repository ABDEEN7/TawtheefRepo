import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';
import { externalUrls } from '../constants/external-urls.const';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.headers.has('X-Skip-Loading') || externalUrls.includes(req.url)) return next(req);

  const loader = inject(LoadingService);
  loader.start();
  return next(req).pipe(finalize(() => loader.stop()));
};
