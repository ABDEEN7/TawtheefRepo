import { CanMatchFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenService } from '../../auth/token.service';
import { routes } from '../../../routes/routes';

/** prevent going to /auth/* if user already has a session */
export const loggedOutOnlyGuard: CanMatchFn = () => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // Session = user_data exists AND (valid access OR refresh valid/unknown)
  if (!tokenService.hasSession()) return true;

  // user already logged-in -> redirect away from /auth/*
  return router.createUrlTree([routes.user.dashboard]);
};
