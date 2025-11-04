import { Routes } from '@angular/router';
import {PublicLayout} from './layouts/public-layout/public-layout';
import {UserLayout} from './layouts/user-layout/user-layout';
import {loggedOutOnlyGuard} from './core/auth/route-guards';

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
      // {
      //   path: 'profile',
      //   // canMatch: [authMatchGuard],
      //   loadChildren: () =>
      //     import('./pages/user-profile/user-profile.module').then(m => m.ProfileModule),
      // },
    ],
  },

  // Fallback
  { path: '**', redirectTo: 'error/404' },
];
