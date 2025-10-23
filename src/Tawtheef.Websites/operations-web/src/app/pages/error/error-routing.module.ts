import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ErrorComponent } from './error.component';
import { Error404Component } from './error404/error404.component';
import { Error500Component } from './error500/error500.component';
import { AccessDeniedComponent } from './access-denied/access-denied.component';
import { UnderConstructionComponent } from './under-construction/under-construction.component';
import { ComeSoonComponent } from './come-soon/come-soon.component';

const routes: Routes = [
  { path: '404', component: Error404Component },
  { path: '500', component: Error500Component },
  { path: 'access-denied', component: AccessDeniedComponent },
  { path: 'under-construction', component: UnderConstructionComponent },
  { path: 'come-soon', component: ComeSoonComponent },
  { path: '', component: ErrorComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ErrorRoutingModule {}
