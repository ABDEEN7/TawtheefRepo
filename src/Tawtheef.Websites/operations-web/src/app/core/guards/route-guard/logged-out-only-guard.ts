import { CanActivateFn, Router } from "@angular/router";
import { inject } from "@angular/core";
import { TokenService } from "../../auth/token.service";
import { NavigationService } from '../../services/navigation.service';

export const loggedOutOnlyGuard: CanActivateFn = () => {
  const tokenService = inject(TokenService);
  const navigationService = inject(NavigationService);
  const router = inject(Router);

  if (!tokenService.hasSession()) return true;

  const mainUserRole = tokenService.getMainUserRole();
  const commands = navigationService.getRoleBasedRoute(mainUserRole);
  return router.createUrlTree(commands);
};

