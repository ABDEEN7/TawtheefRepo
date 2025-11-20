import {userRoutes} from "./dashboard-routes";
import {authRoutes} from "./auth-routes";
import {errorRoutes} from "./error-routes";

export let routes = {
  baseUrl: '',
  home: '/index',
  terms: '/terms',
  privacy: '/privacy',
  auth: {...authRoutes},
  user: {...userRoutes},
  ...errorRoutes,
};
