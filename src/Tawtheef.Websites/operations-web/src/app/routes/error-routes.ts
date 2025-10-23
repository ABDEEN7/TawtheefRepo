export const errorRoutes = {
  error: '/error/',
  get underConstruction() {
    return this.error + 'under-construction';
  },
  get error500() {
    return this.error + '500';
  },
  get comingSoon() {
    return this.error + 'coming-soon';
  },
  get error404() {
    return this.error + '404';
  },
  get accessDenied() {
    return this.error + 'access-denied';
  },
}
