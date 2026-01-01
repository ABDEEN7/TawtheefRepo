import {Routes} from '@angular/router';
import {Layout as AdminLayout} from './layouts/admin/layout/layout';
import {Layout as EmployeeLayout} from './layouts/employee/layout/layout';
import {loggedOutOnlyGuard} from './core/guards/route-guard/logged-out-only-guard';
import {authGuard} from './core/guards/route-guard/auth-guard';
import {SystemRoles} from './core/constants/systemRoles';
export const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'auth/login',
      },
      {
        path: 'index',
        pathMatch: 'full',
        redirectTo: 'auth/login',
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
    canActivate: [authGuard], // then make guard CanActivateFn
    data: { roles: [SystemRoles.SystemAdmin] },
    children: [
      { path: '', loadChildren: () => import('./pages/user/admin/admin.module').then(m => m.AdminModule) }
    ]
  },
  {
    path: 'employee',
    component: EmployeeLayout,
    canActivate: [authGuard],
    data: { roles: [SystemRoles.Employee, SystemRoles.OfficeAdmin, SystemRoles.OfficeUser] },
    children: [
      { path: '', loadChildren: () => import('./pages/user/employee/employee.module').then(m => m.EmployeeModule) }
    ]
  },
  // Fallback
  {path: '**', redirectTo: 'error/404'},
]
