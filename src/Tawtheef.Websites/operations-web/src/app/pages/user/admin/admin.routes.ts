import {Routes} from '@angular/router';
import {RolesManagement} from './roles-management/roles-management';
import {Dashboard} from './dashboard/dashboard';
import {UsersManagement} from './users-management/users-management';
import {OfficesManagement} from './offices-management/offices-management';
import {CountriesManagement} from './countries-management/countries-management';
import {permissionGuard} from '../../../core/guards/route-guard/permission-guards';
import {Permissions} from '../../../core/constants/permissions';

export const adminRoutes: Routes = [
  { path: 'dashboard', component: Dashboard, canActivate: [permissionGuard], data: { permissions: [Permissions.Dashboard.View] } },
  { path: 'roles-management', component: RolesManagement, canActivate: [permissionGuard], data: { permissions: [Permissions.Roles.Manage] } },
  { path: 'users-management', component: UsersManagement, canActivate: [permissionGuard], data: { permissions: [Permissions.Users.Manage] } },
  { path: 'offices-management', component: OfficesManagement, canActivate: [permissionGuard], data: { permissions: [Permissions.Offices.Manage] } },
  { path: 'countries-management', component: CountriesManagement, canActivate: [permissionGuard], data: { permissions: [Permissions.Countries.Manage] } },
];
