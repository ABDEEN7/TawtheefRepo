import {Routes} from '@angular/router';
import {Dashboard} from './dashboard/dashboard';
import {ProfileApprovalListPage} from './profile-managment/approval-list/profile-approval-list.page';
import {ProfileApprovalDetailPage} from './profile-managment/approval-detail/profile-approval-detail.page';
import {ProfileDistributionPage} from './profile-managment/distribution/profile-distribution.page';
import {JobInvitationSummary} from './job-managment/job-invitation-summary/job-invitation-summary';
import {permissionGuard} from '../../../core/guards/route-guard/permission-guards';
import {MajorsSkillsManagement} from './majors-skills-management/majors-skills-management';

export const employeeRoutes: Routes = [
  {
    path: 'dashboard',
    canActivate: [permissionGuard],
    //data: { permissions: ['dashboard.view'] },
    component: Dashboard
  },
  {
    path: 'approval-profile',
    canActivate: [permissionGuard],
    //data: { permissions: ['profile.approval.view'] },
    component: ProfileApprovalListPage
  },
  {
    path: 'approval-profile/:profileId',
    canActivate: [permissionGuard],
    //data: { permissions: ['profile.approval.details.view'] },
    component: ProfileApprovalDetailPage
  },
  {
    path: 'approval-profile/:profileId/review',
    canActivate: [permissionGuard],
    //data: { permissions: ['profile.approval.details.review'] },
    component: ProfileApprovalDetailPage
  },
  {
    path: 'approval-profile/:profileId/changes',
    canActivate: [permissionGuard],
    //data: { permissions: ['profile.approval.details.changes'] },
    component: ProfileApprovalDetailPage
  },
  {
    path: 'profile-distribution',
    canActivate: [permissionGuard],
    //data: { permissions: ['profile.distribution.view'] },
    component: ProfileDistributionPage
  },
  {
    path: 'job-invitation-summary',
    canActivate: [permissionGuard],
    //data: { permissions: ['job.invitation.view'] },
    component: JobInvitationSummary
  },
  {
    path: 'jobs',
    canActivate: [permissionGuard],
    //data: { permissions: ['job.list.view'] },
    loadChildren: () => import('../../job/jobs.module').then(m => m.JobsModule),
  },
  { path: 'majors-skills-management',
    canActivate: [permissionGuard],
    //data: { permissions: ['major-skill.management'] },
    component: MajorsSkillsManagement
  },
];
