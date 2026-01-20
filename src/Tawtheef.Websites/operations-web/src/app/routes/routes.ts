import {authRoutes} from "./auth-routes";
import {errorRoutes} from "./error-routes";
import {adminRoutes} from './admin-routes';
import {employeeRoutes} from './employee-routes';
import {SystemRoles} from '../core/constants/systemRoles';

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
    switch (role) {
      case SystemRoles.SystemAdmin:
        return this.admin.dashboard;
      case SystemRoles.Employee:
      case SystemRoles.OfficeAdmin:
      case SystemRoles.OfficeUser:
      case SystemRoles.DepartmentManager:
        return this.employee.dashboard;
      default:
        return '/';
    }
  },
  settings(role: string) {
    switch (role) {
      case SystemRoles.SystemAdmin:
        return this.admin.settings;
      case SystemRoles.Employee:
      case SystemRoles.OfficeAdmin:
      case SystemRoles.OfficeUser:
      case SystemRoles.DepartmentManager:
        return this.employee.settings;
      default:
        return '/';
    }
  },
};
