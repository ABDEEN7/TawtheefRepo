import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { Select } from 'primeng/select';
import { Tooltip } from 'primeng/tooltip';

import { PaginationComponent } from '../../../../../../../shared/components/pagination/pagination.component';
import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { InterviewAxesCriteriaStore } from '../../interview-axes-criteria.store';
import { InterviewAxesCriteriaFacade } from '../../interview-axes-criteria.facade';

@Component({
  selector: 'app-criteria-tab',
  standalone: true,
  templateUrl: './criteria-tab.html',
  styleUrls: ['../../interview-axes-criteria-actions.scss'],
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    InputTextModule,
    PaginationComponent,
    Select,
    ToggleSwitchModule,
    Tooltip,
    HasPermissionDirective
  ]
})
export class CriteriaTabComponent {
  readonly Permissions = Permissions;
  store = inject(InterviewAxesCriteriaStore);
  service = inject(InterviewAxesCriteriaFacade);

  statusOptions = [
    { label: 'INTERVIEW_AXES_CRITERIA.ACTIVE', value: true },
    { label: 'INTERVIEW_AXES_CRITERIA.INACTIVE', value: false },
  ];
}
