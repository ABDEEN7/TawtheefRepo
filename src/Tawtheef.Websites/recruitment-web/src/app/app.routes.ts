import { Routes } from '@angular/router';
import {UserLayout} from './layouts/internal/user-layout/user-layout';
import {PublicLayout} from './layouts/public/public-layout/public-layout';
import {authGuard, authMatchGuard, loggedOutOnlyGuard} from './core/guards/route-guards';
import {CandidateDashboard} from './pages/candidate-dashboard/candidate-dashboard';

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
        path: 'user',
        // canMatch: [authMatchGuard],
        loadChildren: () =>
          import('./pages/user/user.module').then(m => m.UserModule),
      },
      {
        path: 'candidate-dashboard',
        loadComponent: () =>
          import('./pages/candidate-dashboard/candidate-dashboard').then(
            c => c.CandidateDashboard
          )
      }
    ],
  },

  // Fallback
  { path: '**', redirectTo: 'error/404' },
];
