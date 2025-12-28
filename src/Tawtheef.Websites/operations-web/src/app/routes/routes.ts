import {authRoutes} from "./auth-routes";
import {errorRoutes} from "./error-routes";
import {adminRoutes} from './admin-routes';
import {employeeRoutes} from './employee-routes';
import {Roles} from '../core/constants/roles';

export let routes = {
  baseUrl: '',
  home: '/index',
  terms: '/terms',
  privacy: '/privacy',
  admin: {...adminRoutes},
  employee: {...employeeRoutes},
  auth:{...authRoutes},
  ...errorRoutes,
  dashboard(role: string) {
    switch (role.toLowerCase()) {
      case Roles.SystemAdmin:
        return this.admin.dashboard;
      case Roles.Employee:
        return this.employee.dashboard;
      default:
        return this.employee.dashboard;
    }
  },
  settings(role: string) {
    switch (role.toLowerCase()) {
      case Roles.SystemAdmin:
        return this.admin.settings;
      case Roles.Employee:
        return this.employee.settings;
      default:
        return this.employee.settings;
    }
  },
};
