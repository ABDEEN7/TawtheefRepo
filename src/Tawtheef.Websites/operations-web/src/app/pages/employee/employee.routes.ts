import {Routes} from '@angular/router';
import {ProfileApprovalListPage} from './profile-approval/profile-approval-list.page';
import {ProfileApprovalDetailPage} from './profile-approval-detail/profile-approval-detail.page';
import {ProfileDistributionPage} from './profile-distribution/profile-distribution.page';
import {JobInvitationSummary} from '../job-invitation-summary/job-invitation-summary';
import {Dashboard} from './dashboard/dashboard';

export const employeeRoutes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'approval-profile', component: ProfileApprovalListPage },
  { path: 'approval-profile/:profileId', component: ProfileApprovalDetailPage },
  { path: 'profile-distribution', component: ProfileDistributionPage },
  { path: 'job-invitation-summary', component: JobInvitationSummary },
  {
    path: 'jobs',
    loadChildren: () => import('../job/jobs.module').then(m => m.JobsModule),
  },
];
