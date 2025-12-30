import { Routes } from '@angular/router';
import {JobWizardComponent} from './job-wizard/wizard-container/wizard.component';
import {JobListComponent} from './job-list/jobs-list.component';
import {JobDetailsComponent} from './job-details/job-details.component';
import { JobInvitesDetailsComponent } from './job-invites-details/job-invites-details.component';
import { JobApprovalComponent } from './job-approval/job-approval.component';
import { JobPointsConfigPageComponent } from './job-points/job-points-config-page/job-points-config-page.component';
import { JobsReadyApplicationComponent } from './jobs-ready-application/jobs-ready-application.component';

export const jobRoutes: Routes = [
  { path: '', component: JobListComponent },
  { path: 'create', component: JobWizardComponent },
  { path: 'edit/:id', component: JobWizardComponent },
  { path: 'view/:id', component: JobDetailsComponent },
  { path: 'approval-job/:id', component: JobApprovalComponent },
  { path: 'job-points/:id', component: JobPointsConfigPageComponent },
  { path: 'ready-jobs', component: JobsReadyApplicationComponent },
  { path: 'invites/:id', component: JobInvitesDetailsComponent },
];
