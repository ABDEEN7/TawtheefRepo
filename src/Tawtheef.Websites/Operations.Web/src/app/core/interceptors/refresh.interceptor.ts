import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from '@angular/common/http';
import { catchError, switchMap } from 'rxjs/operators';
import {throwError, from} from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class RefreshInterceptor implements HttpInterceptor {
  private refreshing = false;

  constructor(private auth: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    return next.handle(req).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401 && !req.headers.has('X-Skip-Auth')) {
          if (this.refreshing) {
            // wait logic or queue
            return throwError(() => err);
          }
          this.refreshing = true;
          return from(this.auth.refreshToken()).pipe(
            switchMap((tokens: any) => {
              this.auth.setTokens({
                accessToken: tokens.access,
                refreshToken: tokens.refreshToken,
                expiresAt: tokens.expiresAt,
              });
              const cloned = req.clone({ setHeaders: { Authorization: `Bearer ${tokens.access}` } });
              this.refreshing = false;
              return next.handle(cloned);
            })
          );
        }
        return throwError(() => err);
      })
    );
  }
}
