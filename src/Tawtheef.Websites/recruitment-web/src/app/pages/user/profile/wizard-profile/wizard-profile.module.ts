import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {WizardProfileComponent} from './wizard-profile.component';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {StepPrereqComponent} from '../components/profile-steps/step-first-info/step-prereq.component';
import {StepPersonalComponent} from '../components/profile-steps/step-personal/step-personal.component';
import {StepContactComponent} from '../components/profile-steps/step-contact/step-contact.component';
import {StepDegreeComponent} from '../components/profile-steps/step-degree/step-degree.component';
import {StepExperienceComponent} from '../components/profile-steps/step-experience/step-experience.component';
import {StepAchievementsComponent} from '../components/profile-steps/step-achievements/step-achievements.component';
import {StepSkillsComponent} from '../components/profile-steps/step-skills/step-skills.component';
import {StepLanguagesComponent} from '../components/profile-steps/step-languages/step-languages.component';
import {StepAttachmentsComponent} from '../components/profile-steps/step-attachments/step-attachments.component';
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
import {ImageCropperComponent} from 'ngx-image-cropper';
import {NgxIntlTelInputModule} from 'ngx-intl-tel-input';
import {TextareaModule} from 'primeng/textarea';
import {FaDirArrowDirective} from '../../../../shared/directives/dir-arrow.directive';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {ProfileComponentsModule} from '../components/profile-components.module';
import {PROFILE_WRITE_MODE} from './services/profile-write-mode.token';
import {ProfileService} from './services/profile.service';
import {ProfileDataService} from './services/profile-data.service';

@NgModule({
  declarations: [
    WizardProfileComponent,
    StepReviewComponent,
  ],
  providers:[
    DialogService,
    ProfileDataService,
    ProfileService,
    { provide: PROFILE_WRITE_MODE, useValue: 'create' },
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
    TextareaModule,
    Tooltip,
    FaDirArrowDirective,
    ImageCropperComponent,
    I18nNamespaceDirective,
    NgxIntlTelInputModule,
    ProfileComponentsModule
  ]
})
export class WizardProfileModule {}
