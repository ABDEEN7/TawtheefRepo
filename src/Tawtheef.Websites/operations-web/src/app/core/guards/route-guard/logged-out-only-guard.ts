import {CanMatchFn, Router} from "@angular/router";
import {inject} from "@angular/core";
import {AuthStateService} from "../../auth/auth-state.service";
import {TokenService} from "../../auth/token.service";
import {routes} from "../../../routes/routes";
import {Roles} from '../../constants/roles';

/** optional: prevent going to /auth/* if already logged in */
export const loggedOutOnlyGuard: CanMatchFn = () => {
  const auth = inject(AuthStateService);
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const rawRoles = tokenService.getRolesFromToken(tokenService.getToken() || '');
  const mainSystemRole =
    rawRoles.includes(Roles.SystemAdmin) ? Roles.SystemAdmin
      : rawRoles.includes(Roles.Employee) ? Roles.Employee
        : '';
  return auth.isAuthenticated(true) ? router.createUrlTree([routes.dashboard(mainSystemRole)]) : true;
};
