import {errorRoutes} from './error-routes';

export const userRoutes = {
  user: '/user/',
  get dashboard(){
    return this.user + 'dashboard';
  },
  get profileWizard(){
    return this.user + 'wizard-profile';
  }
};
