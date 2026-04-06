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
    path: 'notifications',
    loadComponent: () => import('../notifications/notifications.page').then(m => m.NotificationsPage)
  },
  {
    path: 'admin-dashboard',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Dashboard.View] },
    loadComponent: () => import('./admin-dashboard/admin-dashboard').then(m => m.AdminDashboard)
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
    path: 'minister-office-management',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.MinisterOffice.View] },
    loadComponent: () => import('./minister-office-management/minister-office-management.page').then(m => m.MinisterOfficeManagementPage)
  },
  {
    path: 'candidate-users-management/:profileId/view',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.CandidateUsers.View] },
    loadComponent: () => import('./candidate-users-management/components/candidate-profile-summary/candidate-profile-summary.component').then(m => m.CandidateProfileSummaryComponent)
  },
  {
    path: 'office-users-management',
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.OfficeUsers.View] },
    loadComponent: () => import('./office-users-management/office-users-management.page').then(m => m.OfficeUsersManagementPage)
  },
  {
    path: "roles-management",
    loadComponent: () => import('./roles-management/roles-management').then(m => m.RolesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Roles.Manage] },
  },
  {
    path: "users-management",
    loadComponent: () => import('./users-management/users-management').then(m => m.UsersManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Users.Manage] },
  },
  {
    path: "profile-logs",
    loadComponent: () => import('./profile-logs/profile-logs').then(m => m.ProfileLogsComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileLogs.View] },
  },
  {
    path: "offices-management",
    loadComponent: () => import('./offices-management/offices-management').then(m => m.OfficesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Offices.Manage] },
  },
  {
    path: "countries-management",
    loadComponent: () => import('./countries-management/countries-management').then(m => m.CountriesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Countries.Manage] },
  },
  {
    path: "languages-management",
    loadComponent: () => import('./languages-management/languages-management').then(m => m.LanguagesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Languages.Manage] },
  },
  {
    path: "target-entities-management",
    loadComponent: () => import('./target-entities-management/target-entities-management').then(m => m.TargetEntitiesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.TargetEntities.Manage] },
  },
  {
    path: "religions-management",
    loadComponent: () => import('./religions-management/religions-management').then(m => m.ReligionsManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Religions.Manage] },
  },
  {
    path: "universities-management",
    loadComponent: () => import('./universities-management/universities-management').then(m => m.UniversitiesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Universities.Manage] },
  },
  {
    path: "job-points-configuration",
    loadComponent: () => import('./job-points-configuration/job-points-configuration.component').then(m => m.JobPointsConfigurationComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.JobPoints.Manage] },
  },
  {
    path: "job-category-candidate-settings",
    loadComponent: () => import('./job-category-candidate-settings/job-category-candidate-settings.component').then(m => m.JobCategoryCandidateSettingsComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Jobs.Manage] },
  },
  {
    path: "job-titles-management",
    loadComponent: () => import('./job-titles-management/job-titles-management.component').then(m => m.JobTitlesManagementComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Jobs.Manage] },
  },
  {
    path: "system-admin-logs",
    loadComponent: () => import('./system-admin-logs/system-admin-logs').then(m => m.SystemAdminLogsComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileLogs.View] },
  },


  {
    path: "dashboard",
    loadComponent: () => import('./admin-dashboard/admin-dashboard').then(m => m.AdminDashboard),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Dashboard.View] },
  },
  {
    path: "roles-management",
    loadComponent: () => import('./roles-management/roles-management').then(m => m.RolesManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Roles.Manage] },
  },
  {
    path: "system-admin-logs",
    loadComponent: () => import('./system-admin-logs/system-admin-logs').then(m => m.SystemAdminLogsComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileLogs.View] },
  },
  {
    path: "users-management",
    loadComponent: () => import('./users-management/users-management').then(m => m.UsersManagement),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Users.Manage] },
  },
  {
    path: "home-content-management",
    loadComponent: () => import('./home-content-management/home-content-management.component').then(m => m.HomeContentManagementComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.HomeContent.Manage] },
  },
];
