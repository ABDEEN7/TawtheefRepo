import { CanActivateChildFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthStateService } from '../../auth/auth-state.service';
import { TokenService } from '../../auth/token.service';
import { routes } from '../../../routes/routes';
import { map } from 'rxjs/operators';

export const authGuard: CanActivateChildFn = (_route, state) => {
  const authState = inject(AuthStateService);
  const tokenService = inject(TokenService);
  const router = inject(Router);

  return authState.ensureAuth$(state.url).pipe(
    map((authResult) => {
      if (authResult !== true) return authResult;

      const requiredRoles = (_route.data?.['roles'] as string[] | undefined) ?? [];
      if (requiredRoles.length === 0) return true;

      const role = tokenService.getRolesFromToken(tokenService.getToken() || '');
      const ok = requiredRoles.some((r) => role.includes(r));
      return ok ? true : router.createUrlTree([routes.accessDenied]);
    }),
  );
};
