import {CanMatchFn, Router} from "@angular/router";
import {inject} from "@angular/core";
import {AuthStateService} from "../../auth/auth-state.service";
import {TokenService} from "../../auth/token.service";
import {routes} from "../../../routes/routes";

/** optional: prevent going to /auth/* if already logged in */
export const loggedOutOnlyGuard: CanMatchFn = () => {
  const auth = inject(AuthStateService);
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const rawRole = tokenService.getRoleFromToken(tokenService.getToken() || '');
  const role = (rawRole || '').toString().toLowerCase();
  return auth.isAuthenticated(true) ? router.createUrlTree([routes.dashboard(role)]) : true;
};
