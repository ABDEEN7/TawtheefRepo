import {NgModule} from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';
import {JobDetailsComponent} from './job-details/job-details.component';
import {StepperComponent} from './job-wizard/stepper/stepper.component';
import {JobBasicModalComponent} from './modals/basics-step-modal/job-basic-modal.component';
import {ConditionsStepComponent} from './job-wizard/wizard-steps/conditions-step.component/conditions-step.component';
import {ReviewStepComponent} from './job-wizard/wizard-steps/review-step.component/review-step.component';
import {SkillsStepComponent} from './job-wizard/wizard-steps/skills-step.component/skills-step.component';
import {ConfirmApplyModalComponent} from './modals/confirm-apply-modal/confirm-apply-modal.component';
import {JobService} from './services/job.service';
import {ProgressBarComponent} from './job-wizard/progress-bar/progress-bar';
import {JobListComponent} from './job-list/jobs-list.component';
import {JobWizardComponent} from './job-wizard/wizard-container/wizard.component';
import {jobRoutes} from './job.routes';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {TranslatePipe} from '@ngx-translate/core';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {DialogService, DynamicDialogModule} from 'primeng/dynamicdialog';
import {SelectModule} from 'primeng/select';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {DatePickerModule} from 'primeng/datepicker';
import {PaginationComponent} from '../../../../shared/components/pagination/pagination.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { Scroller } from "primeng/scroller";
import { OverviewStepComponent } from './job-wizard/wizard-steps/overview-step.component.ts/overview-step.component';
import { QualificationsStepComponent } from './job-wizard/wizard-steps/qualifications-step.component/qualifications-step.component';
import { ResponsibilitiesStepComponent } from './job-wizard/wizard-steps/responsibilities-step.component/responsibilities-step.component';
import { AttachmentStepComponent } from './job-wizard/wizard-steps/attachment-step.component/attachment-step.component';
import { BenefitsStepComponent } from './job-wizard/wizard-steps/benefits-step.component/benefits-step.component';
import { Toast } from "primeng/toast";
import { JobApprovalComponent } from './job-approval/job-approval.component';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { DialogHelperService } from '../../../../core/services/dialog-helper.service';
import { ReviewNoteComponent } from './job-wizard/review-note/review-note.component';
import { FaDirArrowDirective } from '../../../../shared/directives/dir-arrow.directive';import { InputNumber } from 'primeng/inputnumber';
import { JobPointsConfigPageComponent } from './job-points/job-points-config-page/job-points-config-page.component';
import { JobPointsMainElementsComponent } from './job-points/job-points-main-elements/job-points-main-elements.component';
import { JobPointsTabComponent } from './job-points/job-points-points-tab/job-points-tab.component';
import { TabsModule } from 'primeng/tabs';
import { RemoteSelectComponent } from '../../../../shared/components/remote-select/remote-select';
import { JobsReadyApplicationComponent } from './jobs-ready-application/jobs-ready-application.component';
import { TableModule } from 'primeng/table';
import { RadioButtonModule } from 'primeng/radiobutton';
import { JobCandidatesComponent } from './job-candidates/job-candidates.component';
import {Tooltip} from 'primeng/tooltip';
import { JobCandidatesNationalityFilterModalComponent } from './modals/job-candidates-nationality-filter-modal/job-candidates-nationality-filter-modal.component';
import { JobCandidatesNationalityBreakdownDialogComponent } from './modals/job-candidates-nationality-breakdown/job-candidates-nationality-breakdown.dialog.component';
import { JobCandidateProfileDialogComponent } from './modals/job-candidate-profile-dialog/job-candidate-profile.dialog.component';
import { ProgressBarModule } from 'primeng/progressbar';
@NgModule({
  declarations: [
    JobDetailsComponent,
    JobListComponent,
    StepperComponent,
    JobWizardComponent,
    ProgressBarComponent,
    OverviewStepComponent,
    AttachmentStepComponent,
    QualificationsStepComponent,
    BenefitsStepComponent,
    ResponsibilitiesStepComponent,
    ConditionsStepComponent,
    ReviewStepComponent,
    SkillsStepComponent,
    JobBasicModalComponent,
    JobApprovalComponent,
    JobPointsConfigPageComponent,
    JobPointsMainElementsComponent,
    JobPointsTabComponent,
    JobsReadyApplicationComponent,
    JobCandidatesComponent,
    JobCandidateProfileDialogComponent,
    JobCandidatesNationalityFilterModalComponent,
    JobCandidatesNationalityBreakdownDialogComponent,
  ],
  imports: [
    RouterModule.forChild(jobRoutes),
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    I18nNamespaceDirective,
    TranslatePipe,
    CommonModule,
    NgbModalModule,
    DynamicDialogModule,
    SelectModule,
    AutoCompleteModule,
    DatePickerModule,
    PaginationComponent,
    ConfirmApplyModalComponent,
    ReviewNoteComponent,
    NgOptimizedImage,
    MultiSelectModule,
    InputNumber,
    Scroller,
    Toast,
    ConfirmDialog,
    FaDirArrowDirective,
    TabsModule,
    RemoteSelectComponent,
    TableModule,
    RadioButtonModule,
    ProgressBarModule,
    Tooltip
  ],
  exports: [
    JobDetailsComponent,
    JobListComponent,
    JobWizardComponent,
    JobApprovalComponent,
    ConfirmApplyModalComponent,
    JobPointsMainElementsComponent,
    JobPointsTabComponent,
    ProgressBarComponent
  ],
  providers: [
    JobService,
    DialogService,
    ConfirmationService,
    DialogHelperService
  ]
})
export class JobsModule {
}
