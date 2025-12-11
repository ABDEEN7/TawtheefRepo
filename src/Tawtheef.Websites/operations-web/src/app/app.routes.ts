import {Routes} from '@angular/router';
import {authGuard, loggedOutOnlyGuard} from './core/auth/route-guards';
import {Layout as AdminLayout} from './layouts/admin/layout/layout';
import {Layout as EmployeeLayout} from './layouts/employee/layout/layout';
export const routes: Routes = [
  {
    path: '',
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
    path: 'admin',
    component: AdminLayout,
    loadChildren: () => import('./pages/admin/admin.module').then((m) => m.AdminModule),
    canActivate: [authGuard],
    // data: { roles: ['admin'] }
  },
  {
    path: 'employee',
    component: EmployeeLayout,
    loadChildren: () => import('./pages/employee/employee.module').then((m) => m.EmployeeModule),
    canActivate: [authGuard],
    data: { roles: ['employee'] }
  },
  // Fallback
  {path: '**', redirectTo: 'error/404'},
]
