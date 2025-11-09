import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  {
    path: 'wizard-profile',
    loadChildren: () => import('./wizard-profile/wizard-profile.module').then(m => m.WizardProfileModule),
  },
];
