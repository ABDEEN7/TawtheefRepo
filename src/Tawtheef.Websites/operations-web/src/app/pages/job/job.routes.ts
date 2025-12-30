import { Routes } from '@angular/router';
import {JobWizardComponent} from './job-wizard/wizard-container/wizard.component';
import {JobListComponent} from './job-list/jobs-list.component';
import {JobDetailsComponent} from './job-details/job-details.component';
import { JobInvitesDetailsComponent } from './job-invites-details/job-invites-details.component';
import { JobApprovalComponent } from './job-approval/job-approval.component';
import { JobPointsConfigPageComponent } from './job-points/job-points-config-page/job-points-config-page.component';
import { JobsReadyApplicationComponent } from './jobs-ready-application/jobs-ready-application.component';
import {permissionGuard} from '../../core/guards/route-guard/permission-guards';
import {Permissions} from '../../core/constants/permissions';

export const jobRoutes: Routes = [
  { path: '', component: JobListComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.Jobs.View] } },
  { path: 'create', component: JobWizardComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.Jobs.Manage] } },
  { path: 'edit/:id', component: JobWizardComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.Jobs.Manage] } },
  { path: 'view/:id', component: JobDetailsComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.Jobs.View] } },
  { path: 'approval-job/:id', component: JobApprovalComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.Jobs.Approve] } },
  { path: 'job-points/:id', component: JobPointsConfigPageComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.Jobs.PointsManage] } },
  { path: 'ready-jobs', component: JobsReadyApplicationComponent },
  { path: 'invites/:id', component: JobInvitesDetailsComponent, canActivate: [permissionGuard], data: { permissions: [Permissions.JobInvitations.View] } },
];
