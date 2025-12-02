import {Routes} from '@angular/router';
import {loggedOutOnlyGuard} from './core/auth/route-guards';
import {ProfileList} from './pages/profile-list/profile-list'
import {JobInvitationSummary} from './pages/job-invitation-summary/job-invitation-summary'
import { UserLayout } from './layouts/internal/user-layout/user-layout';

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
    component: UserLayout,
    // canActivateChild: [authGuard],
    children: [
      {
        path: 'jobs',
        loadChildren: () => import('./pages/job/jobs.module').then(m => m.JobsModule),
      },
      {
        path: 'approval/tasks',
        loadComponent: () =>
          import('./pages/profile-approval-list/profile-approval-list.page').then(m => m.ProfileApprovalListPage),
      },
      {
        path: 'approval',
        loadComponent: () => import('./pages/profile-approval/profile-approval.page').then(m => m.ProfileApprovalPage),
      },
    ],
  },
  {path: 'profile-list', component: ProfileList},
  {path: 'job-invitation-summary', component: JobInvitationSummary},
  // Fallback
  {path: '**', redirectTo: 'error/404'},
];
