import {CanMatchFn, Router} from "@angular/router";
import {inject} from "@angular/core";
import {AuthStateService} from "../../auth/auth-state.service";
import {TokenService} from "../../auth/token.service";
import {routes} from "../../../routes/routes";
import {SystemRoles} from '../../constants/systemRoles';
import {take} from 'rxjs';
import {map} from 'rxjs/operators';

/** optional: prevent going to /auth/* if already logged in */
export const loggedOutOnlyGuard: CanMatchFn = () => {
  const auth = inject(AuthStateService);
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // IMPORTANT: do NOT trigger refresh/login side-effects from this guard.
  return auth.isAuthenticated$.pipe(
    take(1),
    map(isAuth => {
      if (!isAuth) return true;

      const role = tokenService.getMainUserRole();
      const target = routes.dashboard(role);

      return router.createUrlTree([target]);
    })
  );
};
