import { Routes } from '@angular/router';
import {UserLayout} from './layouts/internal/user-layout/user-layout';
import {PublicLayout} from './layouts/public/public-layout/public-layout';
import {authGuard, authMatchGuard, loggedOutOnlyGuard} from './core/guards/route-guards';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayout,
    children: [
      {
        path: '',
        loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule),
      },
      {
        path: 'index',
        loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule),
      },
    ],
  },
  {
    path: '',
    children: [
      {
        path: 'auth',
        canMatch: [loggedOutOnlyGuard],
        loadChildren: () => import('./pages/auth/auth.module').then(m => m.AuthModule),
      },
      {
        path: 'error',
        loadChildren: () => import('./pages/error/error.module').then(m => m.ErrorModule),
      },
    ],
  },
  {
    path: '',
    component: UserLayout,
    // canActivateChild: [authGuard],
    children: [
      {
        path: 'profile',
        // canMatch: [authMatchGuard],
        loadChildren: () =>
          import('./pages/user-profile/user-profile.module').then(m => m.ProfileModule),
      },
      {
        path: 'dashboard',
        // canMatch: [authMatchGuard, profileCompleteGuard],
        loadChildren: () =>
          import('./pages/user-profile/user-profile.module').then(m => m.ProfileModule),
      },
    ],
  },

  // Fallback
  { path: '**', redirectTo: 'error/404' },
];
