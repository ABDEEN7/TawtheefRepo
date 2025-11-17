import {NgModule} from '@angular/core';
import {StaffInvitesDetailsComponent} from './staff-invites-details/staff-invites-details.component';
import {RouterModule} from '@angular/router';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {I18nNamespaceDirective} from '../../shared/directives/i18n-namespace.directive';
import {TranslatePipe} from '@ngx-translate/core';
import {CommonModule} from '@angular/common';
import {NgbModalModule} from '@ng-bootstrap/ng-bootstrap';
import {DynamicDialogModule} from 'primeng/dynamicdialog';
import {SelectModule} from 'primeng/select';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {DatePickerModule} from 'primeng/datepicker';
import {PaginationComponent} from '../../shared/components/pagination/pagination.component';
import {ResidentsModalComponent} from '../job/modals/residents-modal/residents-modal.component';
import {invitesRoutes} from './invites.routes';

@NgModule({
  declarations: [
    StaffInvitesDetailsComponent
  ],
  imports: [
    RouterModule.forChild(invitesRoutes),
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
    ResidentsModalComponent
  ],
  exports: [],
  providers:[]
})
export class InvitesModule {}
