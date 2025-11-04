import {dashboardRoutes} from "./dashboard-routes";
import {authRoutes} from "./auth-routes";
import {errorRoutes} from "./error-routes";

export let routes = {
  baseUrl: '',
  home: '/index',
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
