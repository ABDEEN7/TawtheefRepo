import {errorRoutes} from './error-routes';

export const userRoutes = {
  user: '/user/',
  get dashboard(){
    return this.user + 'dashboard';
  },
  get profileWizard(){
    return this.user + 'wizard-profile';
  },
  get profileSections(){
    return this.user + 'profile-sections';
  },
  get settings(){
    return this.user + 'settings';
  },
  get profileOverview(){
    return this.user + 'profile-overview';
  }
};
