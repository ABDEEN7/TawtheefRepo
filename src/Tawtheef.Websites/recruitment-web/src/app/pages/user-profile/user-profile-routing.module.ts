import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {UserProfileWizard} from './user-profile-wizard/user-profile-wizard';

const routes: Routes = [
  { path: '', component: UserProfileWizard }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserProfileRoutingModule {}
