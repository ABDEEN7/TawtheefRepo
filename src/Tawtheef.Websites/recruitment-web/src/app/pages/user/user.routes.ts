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
    path: 'candidate-exams/execution',
    canActivate: [profileCompleteGuard],
    loadComponent: () =>
      import('./candidate-exams/pages/candidate-exam-execution/candidate-exam-execution.page').then(
        m => m.CandidateExamExecutionPage
      )
  },
  {
    path: 'candidate-exams',
    canActivate: [profileCompleteGuard],
    loadComponent: () =>
      import('./candidate-exams/pages/candidate-exams/candidate-exams.page').then(m => m.CandidateExamsPage)
  },
  {
    path: 'job-details/:invitationId',
    canActivate: [profileCompleteGuard],
    loadComponent: () =>
      import('./job-details/job-details').then(m => m.JobDetails)
  },
  {
    path: 'notifications',
    canActivate: [profileCompleteGuard],
    loadComponent: () =>
      import('./notifications/notifications.page').then(m => m.NotificationsPage)
  },
  {
    path: 'create-profile',
    loadComponent: () =>
      import('./profile/wizard-profile/wizard-profile.component').then(m => m.WizardProfileComponent),
  }
];
