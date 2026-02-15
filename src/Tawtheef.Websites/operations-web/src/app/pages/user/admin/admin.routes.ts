import {Routes} from '@angular/router';
import {RolesManagement} from './roles-management/roles-management';
import {Dashboard} from './dashboard/dashboard';
import {UsersManagement} from './users-management/users-management';
import {OfficesManagement} from './offices-management/offices-management';
import {CountriesManagement} from './countries-management/countries-management';
import {LanguagesManagement} from './languages-management/languages-management';
import {TargetEntitiesManagement} from './target-entities-management/target-entities-management';
import {ReligionsManagement} from './religions-management/religions-management';
import {UniversitiesManagement} from './universities-management/universities-management';
import {ProfileLogsComponent} from './profile-logs/profile-logs';
import {JobPointsConfigurationComponent} from './job-points-configuration/job-points-configuration.component';
import {
  JobCategoryCandidateSettingsComponent
} from './job-category-candidate-settings/job-category-candidate-settings.component';
import {HomeContentManagementComponent} from './home-content-management/home-content-management.component';
import { JobTitlesManagementComponent } from './job-titles-management/job-titles-management.component';
import {permissionGuard} from '../../../core/guards/route-guard/permission-guards';
import {Permissions} from '../../../core/constants/permissions';

export const adminRoutes: Routes = [
  {
    path: "dashboard",
    component: Dashboard,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Dashboard.View] },
  },
  {
    path: "roles-management",
    component: RolesManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Roles.Manage] },
  },
  {
    path: "profile-logs",
    component: ProfileLogsComponent,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.ProfileLogs.View] },
  },
  {
    path: "users-management",
    component: UsersManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Users.Manage] },
  },
  {
    path: "offices-management",
    component: OfficesManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Offices.Manage] },
  },
  {
    path: "countries-management",
    component: CountriesManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Countries.Manage] },
  },
  {
    path: "languages-management",
    component: LanguagesManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Languages.Manage] },
  },
  {
    path: "target-entities-management",
    component: TargetEntitiesManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.TargetEntities.Manage] },
  },
  {
    path: "religions-management",
    component: ReligionsManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Religions.Manage] },
  },
  {
    path: "universities-management",
    component: UniversitiesManagement,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Universities.Manage] },
  },
  {
    path: "job-points-configuration",
    component: JobPointsConfigurationComponent,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.JobPoints.Manage] },
  },
  {
    path: "job-category-candidate-settings",
    component: JobCategoryCandidateSettingsComponent,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Jobs.Manage] },
  },
  {
    path: "job-titles-management",
    component: JobTitlesManagementComponent,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.Jobs.Manage] },
  },
  {
    path: "home-content-management",
    component: HomeContentManagementComponent,
    canActivate: [permissionGuard],
    data: { permissions: [Permissions.HomeContent.Manage] },
  },
];
