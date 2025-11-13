import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {Login} from './login/login';
import {PopupCallbackComponent} from './popup-callback/popup-callback';

const routes: Routes = [
  { path: '', component: Login },
  { path: 'login', component: Login },
  { path: 'popup-callback', component: PopupCallbackComponent },
  //{ path: 'register', component: RegisterComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule {}
