import { Routes } from '@angular/router';
import { loggedOutOnlyGuard } from './core/guards/route-guard/logged-out-only-guard';
import { authGuard } from './core/guards/route-guard/auth-guard';
import { SystemRoles } from './core/constants/systemRoles';
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
        loadChildren: () => import('./pages/auth/auth.module').then(m => m.AuthModule),
      },
      {
        path: 'error',
        loadChildren: () => import('./pages/error/error.module').then(m => m.ErrorModule),
      },
    ],
  },
  {
    path: 'portal',
    loadComponent: () => import('./layouts/employee/layout/layout').then(m => m.Layout),
    canActivate: [authGuard],
    data: {
      roles: [SystemRoles.Employee, SystemRoles.OfficeAdmin,
      SystemRoles.OfficeUser, SystemRoles.DepartmentManager,
      SystemRoles.HrManager, SystemRoles.SystemAdmin]
    },
    children: [
      { path: '', loadChildren: () => import('./pages/user/employee/employee.module').then(m => m.EmployeeModule) }
    ]
  },
  // Fallback
  { path: '**', redirectTo: 'error/404' },
]
