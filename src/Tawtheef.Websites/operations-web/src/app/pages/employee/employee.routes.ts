import {Routes} from '@angular/router';
import {ProfileApprovalPage} from './profile-approval/profile-approval.page';
import {ProfileDistributionPage} from './profile-distribution/profile-distribution.page';
import {JobInvitationSummary} from '../job-invitation-summary/job-invitation-summary';
import {Dashboard} from './dashboard/dashboard';

export const employeeRoutes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'approval-profile', component: ProfileApprovalPage },
  { path: 'approval-profile', component: ProfileApprovalPage },
  { path: 'profile-distribution', component: ProfileDistributionPage },
  { path: 'job-invitation-summary', component: JobInvitationSummary },
  {
    path: 'jobs',
    loadChildren: () => import('../job/jobs.module').then(m => m.JobsModule),
  },
];
