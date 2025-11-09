import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {WizardProfileComponent} from './wizard-profile.component';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {StepPersonalComponent} from './steps/step-personal/step-personal.component';
import {StepContactComponent} from './steps/step-contact/step-contact.component';
import {StepQualificationComponent} from './steps/step-qualification/step-qualification.component';
import {StepExperienceComponent} from './steps/step-experience/step-experience.component';
import {StepSkillsComponent} from './steps/step-skills/step-skills.component';
import {StepAttachmentsComponent} from './steps/step-attachments/step-attachments.component';
import {StepReviewComponent} from './steps/step-review/step-review.component';
import {TranslatePipe} from '@ngx-translate/core';
import {RouterModule} from '@angular/router';
import {wizardProfileRoutes} from './wizard-profile.routes';
import {TableModule} from 'primeng/table';
import {ChipModule} from 'primeng/chip';
import {FileUploadModule} from 'primeng/fileupload';
import {ButtonModule} from 'primeng/button';
import {InputNumberModule} from 'primeng/inputnumber';
import {InputTextModule} from 'primeng/inputtext';
import {ToggleSwitchModule} from 'primeng/toggleswitch';
import {SelectModule} from 'primeng/select';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {DatePickerModule} from 'primeng/datepicker';
import {DialogService, DynamicDialogModule} from 'primeng/dynamicdialog';
import {Tooltip} from 'primeng/tooltip';
import {FaDirArrowDirective} from '../../../shared/directives/dir-arrow.directive';
import {ImageCropperComponent} from 'ngx-image-cropper';

@NgModule({
  declarations: [
    WizardProfileComponent,
    StepPersonalComponent,
    StepContactComponent,
    StepQualificationComponent,
    StepExperienceComponent,
    StepSkillsComponent,
    StepAttachmentsComponent,
    StepReviewComponent
  ],
  providers:[
    DialogService
  ],
  imports: [
    RouterModule.forChild(wizardProfileRoutes),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModalModule,
    TranslatePipe,
    TableModule,
    ChipModule,
    ToggleSwitchModule,
    DynamicDialogModule,
    SelectModule,
    AutoCompleteModule,
    FileUploadModule,
    ButtonModule,
    InputNumberModule,
    DatePickerModule,
    InputTextModule,
    Tooltip,
    FaDirArrowDirective,
    ImageCropperComponent
  ]
})
export class WizardProfileModule {}
