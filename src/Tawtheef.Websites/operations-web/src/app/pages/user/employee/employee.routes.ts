import {Routes} from '@angular/router';
import {Dashboard} from './dashboard/dashboard';
import {ProfileApprovalListPage} from './profile-managment/approval-list/profile-approval-list.page';
import {ProfileApprovalDetailPage} from './profile-managment/approval-detail/profile-approval-detail.page';
import {ProfileDistributionPage} from './profile-managment/distribution/profile-distribution.page';
import {JobInvitationSummary} from './job-managment/job-invitation-summary/job-invitation-summary';
import {permissionGuard} from '../../../core/guards/route-guard/permission-guards';
import {MajorsSkillsManagementPage} from './majors-skills-management/majors-skills-management';
import {Permissions} from '../../../core/constants/permissions';
import {KawaderPage} from './kawader/kawader.page';
import { JobInvitationSummaryDetailsComponent } from './job-managment/job-invitation-summary-details/job-invitation-summary-details.component';
import {ProfileApprovalWizardPage} from './profile-managment/approval-wizard/profile-approval-wizard.page';
import {OrganizationStructuresPage} from './organization-structures/organization-structures.page';

export const employeeRoutes: Routes = [
  {
    path: 'dashboard',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Dashboard.View] },
    component: Dashboard
  },
  {
    path: 'approval-profile',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileApproval.View] },
    component: ProfileApprovalListPage
  },
  {
    path: 'approval-profile/:profileId/wizard',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileApproval.Review] },
    component: ProfileApprovalWizardPage
  },
  {
    path: 'approval-profile/:profileId/changes',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileApproval.Changes] },
    component: ProfileApprovalDetailPage
  },
  {
    path: 'profile-distribution',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileDistribution.View] },
    component: ProfileDistributionPage
  },
  {
    path: 'job-invitation-summary',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.JobInvitations.View] },
    component: JobInvitationSummary
  },
  {
    path: 'job-invitation-summary-details/:jobId',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.JobInvitations.View] },
    component: JobInvitationSummaryDetailsComponent
  },
  {
    path: 'organization-structures',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.OrganizationStructures.Manage] },
    component: OrganizationStructuresPage
  },
  {
    path: 'jobs',
    canActivate: [permissionGuard],
    loadChildren: () => import('../../job/jobs.module').then(m => m.JobsModule),
  },
  { path: 'majors-skills-management',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.MajorSkills.Manage] },
    component: MajorsSkillsManagementPage
  },
  {
    path: 'kawader',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Kawader.Manage] },
    component: KawaderPage
  },
];
