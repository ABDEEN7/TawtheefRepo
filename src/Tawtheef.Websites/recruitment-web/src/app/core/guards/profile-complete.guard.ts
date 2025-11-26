import {inject, Injector} from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { map } from 'rxjs/operators';
import {routes} from '../../routes/routes';

export const profileCompleteGuard: CanActivateFn = () => {
  const injector = inject(Injector);
  const auth = injector.get(AuthService);
  const router = injector.get(Router);

  const jwtData = auth.decodeBootstrapFromJwt();
  if (jwtData.requiresProfileCompletion) {
    return auth.getAuthBootstrap$().pipe(
      map(response => {
        router.navigate([routes.user.profileWizard], {state: {response}}).then(r => {});
        return false;
      })
    )
  }
  return true;
}
