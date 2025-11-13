import {inject, Injector} from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { map } from 'rxjs/operators';
import {routes} from '../../routes/routes';

export const profileCompleteGuard: CanActivateFn = () => {
  const injector = inject(Injector);
  const auth = injector.get(AuthService);
  const router = injector.get(Router);

  return auth.getAuthBootstrap$().pipe(
    map(b => {
      if (b.requiresProfileCompletion) {
        router.navigate([routes.user.profileWizard], {state: {prefill: b.prefill, missing: b.missingFields}}).then(r => {});
         return false;
      }
      return true;
    })
  );
};
