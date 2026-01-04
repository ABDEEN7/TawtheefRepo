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
  get usersManagement(){
    return this.admin + '/users-management'
  },
  get officesManagement(){
    return this.admin + '/offices-management'
  },
  get countriesManagement(){
    return this.admin + '/countries-management'
  },
  get languagesManagement(){
    return this.admin + '/languages-management'
  },
  get religionsManagement(){
    return this.admin + '/religions-management'
  },
  get universitiesManagement(){
    return this.admin + '/universities-management'
  },
}
