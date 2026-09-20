import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

import { ButtonModule } from 'primeng/button';
import { Tooltip } from 'primeng/tooltip';

import { HasPermissionDirective } from '../../../../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../../../../core/constants/permissions';
import { DialogHelperService } from '../../../../../../../core/services/dialog-helper.service';

import { InterviewTemplatesStore } from '../../interview-templates.store';
import { InterviewTemplatesFacade } from '../../interview-templates.facade';
import { TemplateVersionModel } from '../../models/template-version.model';
import {
  CALCULATION_METHOD_LABELS,
  TEMPLATE_VERSION_STATUS_LABELS,
  TEMPLATE_VERSION_STATUS_PILL,
  TemplateVersionStatus,
} from '../../models/enums';

@Component({
  selector: 'app-template-detail',
  standalone: true,
  templateUrl: './template-detail.html',
  styleUrls: ['./template-detail.scss'],
  imports: [CommonModule, TranslatePipe, ButtonModule, Tooltip, HasPermissionDirective],
})
export class TemplateDetailComponent {
  readonly Permissions = Permissions;
  readonly TemplateVersionStatus = TemplateVersionStatus;

  store = inject(InterviewTemplatesStore);
  service = inject(InterviewTemplatesFacade);
  private dialogHelper = inject(DialogHelperService);

  private localizedLabel(nameAr?: string | null, nameEn?: string | null): string {
    if (!nameAr && !nameEn) return '';
    return this.store.isRtl() ? nameAr || nameEn || '' : nameEn || nameAr || '';
  }

  templateOrgScopeLabel(): string {
    const t = this.store.detailTemplate();
    return this.localizedLabel(t?.organizationScopeNameAr, t?.organizationScopeNameEn);
  }

  templateJobTitleLabel(): string {
    const t = this.store.detailTemplate();
    return this.localizedLabel(t?.jobTitleNameAr, t?.jobTitleNameEn);
  }

  templateDepartmentLabel(): string {
    const t = this.store.detailTemplate();
    return this.localizedLabel(t?.departmentNameAr, t?.departmentNameEn);
  }

  statusLabel(status: TemplateVersionStatus): string {
    return TEMPLATE_VERSION_STATUS_LABELS[status];
  }

  statusPillClass(status: TemplateVersionStatus): string {
    return TEMPLATE_VERSION_STATUS_PILL[status];
  }

  calculationMethodLabel(version: TemplateVersionModel): string {
    return CALCULATION_METHOD_LABELS[version.calculationMethod];
  }

  criterionDisplayName(criterion: { nameAr?: string | null; nameEn?: string | null }): string {
    return this.localizedLabel(criterion.nameAr, criterion.nameEn);
  }

  selectVersion(version: TemplateVersionModel) {
    this.service.loadVersionDetails(version.id);
  }

  createNewVersion() {
    const template = this.store.detailTemplate();
    if (template) this.service.openCreateVersionWizard(template);
  }

  editVersion(version: TemplateVersionModel) {
    const template = this.store.detailTemplate();
    if (template) this.service.openEditVersionWizard(template, version);
  }

  approve(version: TemplateVersionModel) {
    this.dialogHelper
      .openConfirmDialog({
        type: 'submit',
        title: 'INTERVIEW_TEMPLATES.DETAIL.CONFIRM_APPROVE_TITLE',
        description: 'INTERVIEW_TEMPLATES.DETAIL.CONFIRM_APPROVE_DESCRIPTION',
        confirmText: 'INTERVIEW_TEMPLATES.DETAIL.APPROVE',
        cancelText: 'INTERVIEW_TEMPLATES.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_TEMPLATES.DETAIL.DECISION_NOTES_LABEL',
        inputPlaceholder: 'INTERVIEW_TEMPLATES.DETAIL.DECISION_NOTES_PLACEHOLDER',
      })
      ?.onClose.subscribe((notes: string | undefined) => {
        if (notes === undefined) return;
        this.service.approveVersion(version.id, notes?.trim() || null);
      });
  }

  returnForEdit(version: TemplateVersionModel) {
    this.dialogHelper
      .openConfirmDialog({
        type: 'warning',
        title: 'INTERVIEW_TEMPLATES.DETAIL.CONFIRM_RETURN_TITLE',
        description: 'INTERVIEW_TEMPLATES.DETAIL.CONFIRM_RETURN_DESCRIPTION',
        confirmText: 'INTERVIEW_TEMPLATES.DETAIL.RETURN',
        cancelText: 'INTERVIEW_TEMPLATES.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_TEMPLATES.DETAIL.REASON_LABEL',
        inputPlaceholder: 'INTERVIEW_TEMPLATES.DETAIL.REASON_PLACEHOLDER',
      })
      ?.onClose.subscribe((reason: string | undefined) => {
        if (!reason?.trim()) return;
        this.service.returnVersion(version.id, reason.trim());
      });
  }

  cancelVersion(version: TemplateVersionModel) {
    this.dialogHelper
      .openConfirmDialog({
        type: 'delete',
        title: 'INTERVIEW_TEMPLATES.DETAIL.CONFIRM_CANCEL_TITLE',
        description: 'INTERVIEW_TEMPLATES.DETAIL.CONFIRM_CANCEL_DESCRIPTION',
        confirmText: 'INTERVIEW_TEMPLATES.DETAIL.CANCEL_VERSION',
        cancelText: 'INTERVIEW_TEMPLATES.WIZARD.CANCEL',
        showInputField: true,
        inputType: 'textarea',
        inputLabel: 'INTERVIEW_TEMPLATES.DETAIL.REASON_LABEL',
        inputPlaceholder: 'INTERVIEW_TEMPLATES.DETAIL.REASON_PLACEHOLDER',
      })
      ?.onClose.subscribe((reason: string | undefined) => {
        if (!reason?.trim()) return;
        this.service.cancelVersion(version.id, reason.trim());
      });
  }
}
