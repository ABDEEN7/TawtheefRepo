import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private loader: LoadingService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    if (req.headers.has('X-Skip-Loading')) return next.handle(req);
    this.loader.start();
    return next.handle(req).pipe(finalize(() => this.loader.stop()));
  }
}
