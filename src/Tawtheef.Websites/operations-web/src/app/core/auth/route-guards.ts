import { inject } from '@angular/core';
import { Router, CanActivateChildFn, CanMatchFn } from '@angular/router';
import { AuthStateService } from './auth-state.service';
import {routes} from '../../routes/routes';
import {TokenService} from './token.service';

export const authGuard: CanActivateChildFn = (_route, state) => {
  const auth = inject(AuthStateService);
  return auth.ensureAuth(state.url); // true | UrlTree
};

export const authMatchGuard: CanMatchFn = (_route, segments) => {
  const auth = inject(AuthStateService);
  const url = '/' + segments.map(s => s.path).join('/');
  return auth.ensureAuth(url);
};

/** optional: prevent going to /auth/* if already logged in */
export const loggedOutOnlyGuard: CanMatchFn = () => {
  const auth = inject(AuthStateService);
  const tokenService = inject(TokenService);
  const router = inject(Router);
  const rawRole = tokenService.getRoleFromToken(tokenService.getToken() || '');
  const role = (rawRole || '').toString().toLowerCase();
  return auth.isAuthenticated(true) ? router.createUrlTree([routes.dashboard(role)]) : true;
};
