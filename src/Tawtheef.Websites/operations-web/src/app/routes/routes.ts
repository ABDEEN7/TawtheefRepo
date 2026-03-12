import { authRoutes } from "./auth-routes";
import { errorRoutes } from "./error-routes";
import { portalRoutes } from './portal-routes';
import { SystemRoles } from '../core/constants/systemRoles';

export let routes = {
  baseUrl: '',
  home: '/index',
  terms: '/terms',
  privacy: '/privacy',
  portal: { ...portalRoutes },
  auth: { ...authRoutes },
  ...errorRoutes,
  dashboard(role: string) {
    switch (role) {
      case SystemRoles.SystemAdmin:
        return this.portal.adminDashboard;
      case SystemRoles.Employee:
      case SystemRoles.OfficeAdmin:
      case SystemRoles.OfficeUser:
      case SystemRoles.DepartmentManager:
      case SystemRoles.HrManager:
        return this.portal.dashboard;
      default:
        return '/';
    }
  }
};
