import { Routes } from '@angular/router';
import {Dashboard} from './dashboard/dashboard';
import {profileCompleteGuard} from '../../core/guards/profile-complete.guard';

export const userRoutes: Routes = [
  {
    path: 'dashboard',
    canActivate: [profileCompleteGuard],
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
      import('./profile/view/profile-view.page').then(m => m.ProfileViewPage)
  },
  {
    path: 'job-details/:invitationId',
    loadComponent: () =>
      import('./job-details/job-details').then(m => m.JobDetails)
  }
];
