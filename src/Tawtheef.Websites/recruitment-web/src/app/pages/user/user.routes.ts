import { Routes } from '@angular/router';
import {Dashboard} from './dashboard/dashboard';
import {profileCompleteGuard} from '../../core/guards/profile-complete.guard';

export const userRoutes: Routes = [
  {
    path: 'wizard-profile',
    loadChildren: () => import('./wizard-profile/wizard-profile.module').then(m => m.WizardProfileModule),
  },
  {
    path: 'dashboard',
    canMatch: [profileCompleteGuard],
    loadComponent: () => Dashboard
  },
];
