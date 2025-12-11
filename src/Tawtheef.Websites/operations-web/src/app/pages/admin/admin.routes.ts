import {Routes} from '@angular/router';
import {RolesManagement} from './roles-management/roles-management';
import {Dashboard} from './dashboard/dashboard';

export const adminRoutes: Routes = [
  { path: 'dashboard', component: Dashboard },
  { path: 'roles-management', component: RolesManagement },
];
