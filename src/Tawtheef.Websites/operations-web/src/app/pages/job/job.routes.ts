import { Routes } from '@angular/router';
import {JobWizardComponent} from './job-wizard/wizard-container/wizard.component';
import {JobListComponent} from './job-list/jobs-list.component';
import {JobDetailsComponent} from './job-details/job-details.component';
import { JobInvitesDetailsComponent } from './job-invites-details/job-invites-details.component';
import { JobApprovalComponent } from './job-approval/job-approval.component';

export const jobRoutes: Routes = [
  { path: '', component: JobListComponent },
  { path: 'create', component: JobWizardComponent },
  { path: 'edit/:id', component: JobWizardComponent },
  { path: 'view/:id', component: JobDetailsComponent },
  { path: 'approval-job/:id', component: JobApprovalComponent },
  { path: 'invites/:id', component: JobInvitesDetailsComponent },
];
