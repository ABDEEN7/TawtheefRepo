import {Routes} from '@angular/router';
import {RolesManagement} from './roles-management/roles-management';
import {Dashboard} from './dashboard/dashboard';
import {UsersManagement} from './users-management/users-management';
import {OfficesManagement} from './offices-management/offices-management';

export const adminRoutes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'roles-management', component: RolesManagement },
  { path: 'users-management', component: UsersManagement },
  { path: 'offices-management', component: OfficesManagement },
];
