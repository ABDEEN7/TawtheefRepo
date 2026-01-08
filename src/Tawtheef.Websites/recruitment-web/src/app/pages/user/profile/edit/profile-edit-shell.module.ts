import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {TranslatePipe} from '@ngx-translate/core';
import {RouterModule} from '@angular/router';
import {profileEditShellRoutes} from './profile-edit-shell.routes';
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
import {ProfileService} from '../wizard-profile/services/profile.service';
import {PROFILE_WRITE_MODE} from '../wizard-profile/services/profile-write-mode.token';
import {ProfileDataService} from '../wizard-profile/services/profile-data.service';
@NgModule({
  declarations: [
  ],
  providers:[
    DialogService,
    ProfileDataService,
    ProfileService,
    { provide: PROFILE_WRITE_MODE, useValue: 'create' },
  ],
  imports: [
    RouterModule.forChild(profileEditShellRoutes),
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
export class ProfileEditShellModule {}
