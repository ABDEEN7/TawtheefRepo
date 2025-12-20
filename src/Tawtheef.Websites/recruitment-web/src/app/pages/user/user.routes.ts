import { Routes } from '@angular/router';
import {Dashboard} from './dashboard/dashboard';
import {profileCompleteGuard} from '../../core/guards/profile-complete.guard';

export const userRoutes: Routes = [
  {
    path: 'dashboard',
    canMatch: [profileCompleteGuard],
    loadComponent: () => Dashboard
  },
  {
    path: 'wizard-profile',
    loadChildren: () =>
      import('./profile//wizard-profile/wizard-profile.module').then(m => m.WizardProfileModule),
  },
  {
    path: 'profile-overview',
    loadComponent: () =>
      import('./profile/overview/profile-overview.page').then(m => m.ProfileOverviewPage)
  },
  {
    path: 'profile/edit',
    loadChildren: () =>
      import('./profile/edit/profile-edit-shell.module').then(m => m.ProfileEditShellModule),
  }
];
