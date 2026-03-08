import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './login/login';
import { PopupCallbackComponent } from './popup-callback/popup-callback';
import { PendingApprovalComponent } from './pending-approval/pending-approval';

import { loggedOutOnlyGuard } from '../../core/guards/route-guard/logged-out-only-guard';

const routes: Routes = [
  { path: '', component: Login, canActivate: [loggedOutOnlyGuard] },
  { path: 'login', component: Login, canActivate: [loggedOutOnlyGuard] },
  { path: 'popup-callback', component: PopupCallbackComponent, canActivate: [loggedOutOnlyGuard] },
  { path: 'pending-approval', component: PendingApprovalComponent },
  //{ path: 'register', component: RegisterComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
