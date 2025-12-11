export const adminRoutes = {
  admin: '/admin',
  get dashboard(){
    return this.admin + '/dashboard';
  },
  get settings(){
    return this.admin + '/settings';
  },
  get roleManagement(){
    return this.admin + '/roles-management'
  },
}
