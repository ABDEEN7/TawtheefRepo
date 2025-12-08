import {inject, Injector} from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { map, switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import {routes} from '../../routes/routes';

export const profileCompleteGuard: CanActivateFn = () => {
  const injector = inject(Injector);
  const auth = injector.get(AuthService);
  const router = injector.get(Router);

  const jwtData = auth.decodeBootstrapFromJwt();
  if (jwtData.requiresProfileCompletion) {
    return auth.getAuthBootstrap$().pipe(
      switchMap(response => {
        if (response.isComplete) {
          return auth.refreshToken().pipe(
            map(() => true),
            catchError(() => of(true)),
          );
        }
        router.navigate([routes.user.profileWizard], {state: {response}}).then(r => {});
        return of(false);
      })
    )
  }
  return true;
}
