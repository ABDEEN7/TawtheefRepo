import {inject} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivateFn, Router} from '@angular/router';
import {AuthStateService} from '../../auth/auth-state.service';
import {routes} from '../../../routes/routes';
import {AuthService} from '../../auth/auth.service';

export const permissionGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state) => {
  const authState = inject(AuthStateService);
  const authService = inject(AuthService);
  const router = inject(Router);

  // Ensure logged in (no role here; role handled by authGuard)
  const authResult = authState.ensureAuth(state.url);
  if (authResult !== true) return authResult;

  // permissions can be on this route or inherited
  const requiredPerms = collectPermissions(route);
  if (requiredPerms.length === 0) return true;

  // Require ALL permissions collected from the route and its parents
  const ok = authService.hasPermission(requiredPerms, true);
  return ok ? true : router.createUrlTree([routes.accessDenied]);
};

function collectPermissions(route: ActivatedRouteSnapshot): string[] {
  const perms: string[] = [];
  let cur: ActivatedRouteSnapshot | null = route;

  while (cur) {
    const p = cur.data?.['permissions'] as string[] | undefined;
    if (Array.isArray(p) && p.length) perms.push(...p);
    cur = cur.parent;
  }

  return Array.from(new Set(perms.map(x => x.trim()).filter(Boolean)));
}

