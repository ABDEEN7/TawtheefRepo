import { Routes } from '@angular/router';
import {Dashboard} from './dashboard/dashboard';

export const userRoutes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => Dashboard
  },
  {
    path: 'wizard-profile',
    loadChildren: () => import('./wizard-profile/wizard-profile.module').then(m => m.WizardProfileModule),
  },
];
