import {CanMatchFn, Router} from "@angular/router";
import {inject} from "@angular/core";
import {AuthStateService} from "../../auth/auth-state.service";
import {TokenService} from "../../auth/token.service";
import {routes} from "../../../routes/routes";
import {take} from 'rxjs';
import {map} from 'rxjs/operators';

export const loggedOutOnlyGuard: CanMatchFn = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (!tokenService.hasSession()) return true;

  const role = tokenService.getMainUserRole();
  return router.createUrlTree([routes.dashboard(role)]);
};

