import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { map } from 'rxjs/operators';
import { routes } from '../../routes/routes';

export const BOOTSTRAP_KEY = 'profileWizardBootstrap';
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

      // ✅ store for wizard to read
      sessionStorage.setItem(BOOTSTRAP_KEY, JSON.stringify(response));

      // ✅ just redirect with url tree
      return router.createUrlTree([routes.user.profileWizard], {
        queryParams: { fromGuard: 1 },
      });
    }),
  );
};
