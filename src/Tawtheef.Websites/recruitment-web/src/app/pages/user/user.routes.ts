import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { profileCompleteGuard } from '../../core/guards/profile-complete.guard';

export const userRoutes: Routes = [
  {
    path: 'dashboard',
    canActivate: [profileCompleteGuard],
    loadComponent: () => Dashboard
  },
  {
    path: 'profile-overview',
    canActivate: [profileCompleteGuard],
    loadComponent: () =>
      import('./profile/view/profile-view.page').then(m => m.ProfileViewPage)
  },
  {
    path: 'job-details/:invitationId',
    canActivate: [profileCompleteGuard],
    loadComponent: () =>
      import('./job-details/job-details').then(m => m.JobDetails)
  },
  {
    path: 'create-profile',
    loadComponent: () =>
      import('./profile/wizard-profile/wizard-profile.component').then(m => m.WizardProfileComponent),
  }
];
