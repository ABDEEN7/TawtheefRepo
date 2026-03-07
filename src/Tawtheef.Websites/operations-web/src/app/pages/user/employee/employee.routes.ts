import { Routes } from '@angular/router';
import { permissionGuard } from '../../../core/guards/route-guard/permission-guards';
import { Permissions } from '../../../core/constants/permissions';

export const employeeRoutes: Routes = [
  {
    path: 'dashboard',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Dashboard.View] },
    loadComponent: () => import('./dashboard/dashboard').then(m => m.Dashboard)
  },
  {
    path: 'approval-profile',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileApproval.View] },
    loadComponent: () => import('./profile-managment/approval-list/profile-approval-list.page').then(m => m.ProfileApprovalListPage)
  },
  {
    path: 'approval-profile/:profileId/wizard',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileApproval.Review] },
    loadComponent: () => import('./profile-managment/approval-wizard/profile-approval-wizard.page').then(m => m.ProfileApprovalWizardPage)
  },
  {
    path: 'approval-profile/:profileId/changes',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileApproval.Changes] },
    loadComponent: () => import('./profile-managment/approval-detail/profile-approval-detail.page').then(m => m.ProfileApprovalDetailPage)
  },
  {
    path: 'profile-distribution',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileDistribution.View] },
    loadComponent: () => import('./profile-managment/distribution/profile-distribution.page').then(m => m.ProfileDistributionPage)
  },
  {
    path: 'job-invitation-summary',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.JobInvitations.View] },
    loadComponent: () => import('./job-management/job-invitation-summary/job-invitation-summary').then(m => m.JobInvitationSummary)
  },
  {
    path: 'job-invitation-summary-details/:jobId',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.JobInvitations.View] },
    loadComponent: () => import('./job-management/job-invitation-summary-details/job-invitation-summary-details.component').then(m => m.JobInvitationSummaryDetailsComponent)
  },
  {
    path: 'organization-structures',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.OrganizationStructures.Manage] },
    loadComponent: () => import('./organization-structures/organization-structures.page').then(m => m.OrganizationStructuresPage)
  },
  {
    path: 'jobs',
    canActivate: [permissionGuard],
    loadChildren: () => import('./job-management/jobs.module').then(m => m.JobsModule),
  },
  {
    path: 'majors-skills-management',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.MajorSkills.Manage] },
    loadComponent: () => import('./majors-skills-management/majors-skills-management').then(m => m.MajorsSkillsManagementPage)
  },
  {
    path: 'kawader',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Kawader.Manage] },
    loadComponent: () => import('./kawader/kawader.page').then(m => m.KawaderPage)
  },
  {
    path: 'candidate-users-management',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.CandidateUsers.View] },
    loadComponent: () => import('./candidate-users-management/candidate-users-management.page').then(m => m.CandidateUsersManagementPage)
  },
  {
    path: 'office-users-management',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.OfficeUsers.View] },
    loadComponent: () => import('./office-users-management/office-users-management.page').then(m => m.OfficeUsersManagementPage)
  },
];
