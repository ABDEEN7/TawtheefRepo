import { Routes } from '@angular/router';
import {UserLayout} from './layouts/internal/user-layout/user-layout';
import {PublicLayout} from './layouts/public/public-layout/public-layout';
import {loggedOutOnlyGuard} from './core/guards/route-guard/logged-out-only-guard';
import {authGuard} from './core/guards/route-guard/auth-guard';

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
    canActivateChild: [authGuard],
    children: [
      {
        path: 'user',
        loadChildren: () =>
          import('./pages/user/user.module').then(m => m.UserModule),
      }
    ],
  },

  // Fallback
  { path: '**', redirectTo: 'error/404' },
];
