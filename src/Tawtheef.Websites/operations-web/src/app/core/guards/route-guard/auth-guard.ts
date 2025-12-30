import {CanActivateChildFn, Router} from "@angular/router";
import {inject} from "@angular/core";
import {AuthStateService} from "../../auth/auth-state.service";
import {TokenService} from "../../auth/token.service";
import {routes} from "../../../routes/routes";

export const authGuard: CanActivateChildFn = (_route, state) => {
  const authState = inject(AuthStateService);
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // Auth first (login redirect with returnUrl)
  const authResult = authState.ensureAuth(state.url);
  if (authResult !== true) return authResult;

  // Role check (from route data OR fallback to token)
  const requiredRoles = (_route.data?.['roles'] as string[] | undefined) ?? [];
  if (requiredRoles.length === 0) return true;

  const role = tokenService.getRolesFromToken(tokenService.getToken() || '');

  const ok = requiredRoles.some(r => role.includes(r));
  return ok ? true : router.createUrlTree([routes.accessDenied]);
};
