import { NgModule } from '@angular/core';
import {StepPrereqComponent} from './profile-steps/step-first-info/step-prereq.component';
import {StepPersonalComponent} from './profile-steps/step-personal/step-personal.component';
import {StepContactComponent} from './profile-steps/step-contact/step-contact.component';
import {StepDegreeComponent} from './profile-steps/step-degree/step-degree.component';
import {StepExperienceComponent} from './profile-steps/step-experience/step-experience.component';
import {StepSkillsComponent} from './profile-steps/step-skills/step-skills.component';
import {StepLanguagesComponent} from './profile-steps/step-languages/step-languages.component';
import {StepAchievementsComponent} from './profile-steps/step-achievements/step-achievements.component';
import {StepAttachmentsComponent} from './profile-steps/step-attachments/step-attachments.component';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {TranslatePipe} from '@ngx-translate/core';
import {TableModule} from 'primeng/table';
import {ChipModule} from 'primeng/chip';
import {ToggleSwitchModule} from 'primeng/toggleswitch';
import {DialogService, DynamicDialogModule} from 'primeng/dynamicdialog';
import {SelectModule} from 'primeng/select';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {FileUploadModule} from 'primeng/fileupload';
import {ButtonModule} from 'primeng/button';
import {InputNumberModule} from 'primeng/inputnumber';
import {DatePickerModule} from 'primeng/datepicker';
import {InputTextModule} from 'primeng/inputtext';
import {TextareaModule} from 'primeng/textarea';
import {Tooltip} from 'primeng/tooltip';
import {FaDirArrowDirective} from '../../../../shared/directives/dir-arrow.directive';
import {ImageCropperComponent} from 'ngx-image-cropper';
import {I18nNamespaceDirective} from '../../../../shared/directives/i18n-namespace.directive';
import {NgxIntlTelInputModule} from 'ngx-intl-tel-input';
import {RemoteSelectComponent} from "../../../../shared/components/remote-select/remote-select";

@NgModule({
  declarations: [
    StepPrereqComponent,
    StepPersonalComponent,
    StepContactComponent,
    StepDegreeComponent,
    StepExperienceComponent,
    StepSkillsComponent,
    StepLanguagesComponent,
    StepAchievementsComponent,
    StepAttachmentsComponent,
  ],
  providers: [
    DialogService
  ],
  exports: [
    StepPrereqComponent,
    StepPersonalComponent,
    StepContactComponent,
    StepDegreeComponent,
    StepExperienceComponent,
    StepSkillsComponent,
    StepLanguagesComponent,
    StepAchievementsComponent,
    StepAttachmentsComponent,
  ],
    imports: [
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
        RemoteSelectComponent,
    ]
})
export class ProfileComponentsModule {}
