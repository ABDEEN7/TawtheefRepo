import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../auth/token.service';
import { routes } from '../../routes/routes';

export const BOOTSTRAP_KEY = 'profileWizardBootstrap';
export const profileCompleteGuard: CanActivateFn = () => {
  const tokens = inject(TokenService);
  const router = inject(Router);

  if (tokens.isProfileComplete()) {
    return true;
  }

  // ✅ just redirect with url tree
  return router.createUrlTree([routes.user.profileWizard], {
    queryParams: { fromGuard: 1 },
  });
};
