import { Routes } from '@angular/router';
import { permissionGuard } from '../../../core/guards/route-guard/permission-guards';
import { Permissions } from '../../../core/constants/permissions';

export const adminRoutes: Routes = [
  {
    path: "dashboard",
    loadComponent: () => import('./dashboard/dashboard').then(m => m.Dashboard),
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
    path: "profile-logs",
    loadComponent: () => import('./profile-logs/profile-logs').then(m => m.ProfileLogsComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileLogs.View] },
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
    path: "home-content-management",
    loadComponent: () => import('./home-content-management/home-content-management.component').then(m => m.HomeContentManagementComponent),
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.HomeContent.Manage] },
  },
];
