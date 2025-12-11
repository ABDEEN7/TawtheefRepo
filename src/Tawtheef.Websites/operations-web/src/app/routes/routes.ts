import {authRoutes} from "./auth-routes";
import {errorRoutes} from "./error-routes";
import {adminRoutes} from './admin-routes';
import {employeeRoutes} from './employee-routes';

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
      case 'admin':
        return this.admin.dashboard;
      case 'employee':
        return this.employee.dashboard;
      default:
        return this.employee.dashboard;
    }
  },
  settings(role: string) {
    switch (role.toLowerCase()) {
      case 'admin':
        return this.admin.settings;
      case 'employee':
        return this.employee.settings;
      default:
        return this.employee.settings;
    }
  },
};
