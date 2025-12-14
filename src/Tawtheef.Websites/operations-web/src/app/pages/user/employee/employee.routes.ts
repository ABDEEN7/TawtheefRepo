import {Routes} from '@angular/router';
import {Dashboard} from './dashboard/dashboard';
import {ProfileApprovalListPage} from './profile-managment/approval-list/profile-approval-list.page';
import {ProfileApprovalDetailPage} from './profile-managment/approval-detail/profile-approval-detail.page';
import {ProfileDistributionPage} from './profile-managment/distribution/profile-distribution.page';
import {JobInvitationSummary} from './job-managment/job-invitation-summary/job-invitation-summary';

export const employeeRoutes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'approval-profile', component: ProfileApprovalListPage },
  { path: 'approval-profile/:profileId', component: ProfileApprovalDetailPage },
  { path: 'profile-distribution', component: ProfileDistributionPage },
  { path: 'job-invitation-summary', component: JobInvitationSummary },
  {
    path: 'jobs',
    loadChildren: () => import('../../job/jobs.module').then(m => m.JobsModule),
  },
];
