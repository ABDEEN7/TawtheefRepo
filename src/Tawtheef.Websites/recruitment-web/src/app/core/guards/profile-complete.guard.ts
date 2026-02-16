import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { map } from 'rxjs/operators';
import { routes } from '../../routes/routes';

export const profileCompleteGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const jwtData = auth.decodeBootstrapFromJwt();
  if (!jwtData.requiresProfileCompletion) {
    return true;
  }

  return auth.getAuthBootstrap$().pipe(
    map((response) => {
      if (response.isComplete) {
        return true;
      }

      return router.createUrlTree([routes.user.profileWizard], {
        state: { response },
      });
    }),
  );
};
