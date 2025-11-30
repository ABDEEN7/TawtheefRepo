import {dashboardRoutes} from "./dashboard-routes";
import {authRoutes} from "./auth-routes";
import {errorRoutes} from "./error-routes";

export let routes = {
  baseUrl: '',
  home: '/index',
  terms: '/terms',
  privacy: '/privacy',
  dashboard(role: string) {
    switch (role.toLowerCase()) {
      case 'admin':
        return this.adminDashboard;
      default:
        return '/';
    }
  },
  ...dashboardRoutes,
  auth:{...authRoutes},
  ...errorRoutes,
};
