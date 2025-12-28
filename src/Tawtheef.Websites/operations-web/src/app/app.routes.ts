import {Routes} from '@angular/router';
import {Layout as AdminLayout} from './layouts/admin/layout/layout';
import {Layout as EmployeeLayout} from './layouts/employee/layout/layout';
import {loggedOutOnlyGuard} from './core/guards/route-guard/logged-out-only-guard';
import {authGuard} from './core/guards/route-guard/auth-guard';
import {Roles} from './core/constants/roles';
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
    path: '',
    component: AdminLayout,
    canActivate: [authGuard],
    data: { roles: [Roles.SystemAdmin] },
    children:[
      { path: 'admin', loadChildren: () => import('./pages/user/admin/admin.module').then((m) => m.AdminModule),}
    ]
  },
  {
    path: '',
    component: EmployeeLayout,
    canActivate: [authGuard],
    data: { roles: [Roles.Employee] },
    children:[
      { path: 'employee', loadChildren: () => import('./pages/user/employee/employee.module').then((m) => m.EmployeeModule),}
    ]
  },
  // Fallback
  {path: '**', redirectTo: 'error/404'},
]
