import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadChildren: () => import('./pages/home/home.module').then((m) => m.HomeModule) },
  { path: 'auth', loadChildren: () => import('./features//auth/auth.module').then((m) => m.AuthModule) },
  { path: 'error', loadChildren: () => import('./pages/error/error.module').then((m) => m.ErrorModule) },
  { path: '**', redirectTo: 'error/404'},
];
