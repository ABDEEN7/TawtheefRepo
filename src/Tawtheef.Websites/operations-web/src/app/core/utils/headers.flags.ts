export const HDR = {
  SkipAuth: 'X-Skip-Auth',         // don't attach access token
  SkipError: 'X-Skip-Error',       // don't show toasts
  SkipRefresh: 'X-Skip-Refresh',   // don't attempt refresh on 401
  Retried: 'X-Retried-After-Refresh', // original request retried once after refresh
  LogoutFlow: 'X-Logout-Flow',     // this request belongs to logout sequence
};
