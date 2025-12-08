import {Routes} from '@angular/router';
import {loggedOutOnlyGuard} from './core/auth/route-guards';
import {JobInvitationSummary} from './pages/job-invitation-summary/job-invitation-summary';
import { UserLayout } from './layouts/internal/user-layout/user-layout';
import {ProfileApprovalListPage} from './pages/profile-approval-list/profile-approval-list.page';
import {ProfileApprovalPage} from './pages/profile-approval/profile-approval.page';
import {ProfileDistributionPage} from './pages/profile-distribution/profile-distribution.page';

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
      { path: 'approval/tasks', component: ProfileApprovalListPage },
      { path: 'approval', component: ProfileApprovalPage },
      { path: 'profile-distribution', component: ProfileDistributionPage },
      { path: 'job-invitation-summary', component: JobInvitationSummary },
    ],
  },
  // Fallback
  {path: '**', redirectTo: 'error/404'},
]
