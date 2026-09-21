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

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { InterviewTemplatesStore } from '../../interview-templates.store';
import { InterviewTemplatesFacade } from '../../interview-templates.facade';
import { TemplateModel } from '../../models/template.model';
import {
  TEMPLATE_VERSION_STATUS_LABELS,
  TEMPLATE_VERSION_STATUS_PILL,
  TemplateVersionStatus,
} from '../../models/enums';

@Component({
  selector: 'app-templates-list',
  standalone: true,
  templateUrl: './templates-list.html',
  imports: [
    CommonModule,
    FormsModule,
    TranslatePipe,
    TableModule,
    ButtonModule,
    InputTextModule,
    Select,
    ToggleSwitchModule,
    Tooltip,
    HasPermissionDirective,
  ],
})
export class TemplatesListComponent {
  readonly Permissions = Permissions;
  store = inject(InterviewTemplatesStore);
  service = inject(InterviewTemplatesFacade);

  statusOptions = [
    { label: 'INTERVIEW_TEMPLATES.ACTIVE', value: true },
    { label: 'INTERVIEW_TEMPLATES.INACTIVE', value: false },
  ];

  openDetail(template: TemplateModel) {
    this.service.openDetail(template);
  }

  versionStatusLabel(status: TemplateVersionStatus): string {
    return TEMPLATE_VERSION_STATUS_LABELS[status];
  }

  versionStatusPillClass(status: TemplateVersionStatus): string {
    return TEMPLATE_VERSION_STATUS_PILL[status];
  }
}
