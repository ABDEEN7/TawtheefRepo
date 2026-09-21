import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

import { Select } from 'primeng/select';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { Textarea } from 'primeng/textarea';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

import { InterviewTemplatesStore, WizardStep } from '../../interview-templates.store';
import { InterviewTemplatesFacade } from '../../interview-templates.facade';
import { CALCULATION_METHOD_LABELS, CalculationMethod } from '../../models/enums';
import { WizardAxisDraft, WizardCriterionDraft, CriterionDraftMode } from '../../models/wizard-draft.model';

interface SelectOption {
  label: string;
  value: string;
}

@Component({
  selector: 'app-template-wizard',
  standalone: true,
  templateUrl: './template-wizard.html',
  styleUrls: ['./template-wizard.scss'],
  imports: [CommonModule, FormsModule, TranslatePipe, Select, ToggleSwitchModule, Textarea, InputTextModule, ButtonModule],
})
export class TemplateWizardComponent {
  store = inject(InterviewTemplatesStore);
  service = inject(InterviewTemplatesFacade);

  readonly steps: WizardStep[] = [1, 2, 3, 4];
  readonly stepLabels: Record<WizardStep, string> = {
    1: 'INTERVIEW_TEMPLATES.WIZARD.STEP1_LABEL',
    2: 'INTERVIEW_TEMPLATES.WIZARD.STEP2_LABEL',
    3: 'INTERVIEW_TEMPLATES.WIZARD.STEP3_LABEL',
    4: 'INTERVIEW_TEMPLATES.WIZARD.STEP4_LABEL',
  };

  readonly calculationMethodOptions = [
    { label: CALCULATION_METHOD_LABELS[CalculationMethod.AverageOfEvaluators], value: CalculationMethod.AverageOfEvaluators },
    { label: CALCULATION_METHOD_LABELS[CalculationMethod.WeightedByRole], value: CalculationMethod.WeightedByRole },
    { label: CALCULATION_METHOD_LABELS[CalculationMethod.ChairmanDecides], value: CalculationMethod.ChairmanDecides },
  ];

  readonly criterionModeOptions: { label: string; value: CriterionDraftMode }[] = [
    { label: 'INTERVIEW_TEMPLATES.WIZARD.SOURCE_BANK', value: 'bank' },
    { label: 'INTERVIEW_TEMPLATES.WIZARD.SOURCE_CUSTOM', value: 'custom' },
  ];

  get isNewVersionMode(): boolean {
    return this.store.wizardMode() === 'new-version';
  }

  get isEditMode(): boolean {
    return this.store.wizardMode() === 'edit-version';
  }

  wizardTitleKey(): string {
    if (this.isEditMode) return 'INTERVIEW_TEMPLATES.WIZARD.EDIT_VERSION_TITLE';
    if (this.isNewVersionMode) return 'INTERVIEW_TEMPLATES.WIZARD.NEW_VERSION_TITLE';
    return 'INTERVIEW_TEMPLATES.WIZARD.CREATE_TITLE';
  }

  goToStep(step: WizardStep) {
    this.service.goToStep(step);
  }

  submit() {
    void this.service.submitWizard();
  }

  private localizedLabel(nameAr: string, nameEn?: string | null): string {
    return this.store.isRtl() ? nameAr : nameEn || nameAr;
  }

  orgScopeOptions(): SelectOption[] {
    return this.store.lookups().organizationScopes.map((o) => ({ label: this.localizedLabel(o.nameAr, o.nameEn), value: o.id }));
  }

  jobTitleOptions(): SelectOption[] {
    return this.store.lookups().jobTitles.map((o) => ({ label: this.localizedLabel(o.nameAr, o.nameEn), value: o.id }));
  }

  departmentOptions(): SelectOption[] {
    return this.store.lookups().departments.map((o) => ({ label: this.localizedLabel(o.nameAr, o.nameEn), value: o.id }));
  }

  orgScopeLabel(): string {
    const info = this.store.wizardInfo();
    const match = this.store.lookups().organizationScopes.find((o) => o.id === info.organizationScopeId);
    return match ? this.localizedLabel(match.nameAr, match.nameEn) : '';
  }

  jobTitleLabel(): string {
    const info = this.store.wizardInfo();
    const match = this.store.lookups().jobTitles.find((o) => o.id === info.jobTitleId);
    return match ? this.localizedLabel(match.nameAr, match.nameEn) : '';
  }

  departmentLabel(): string {
    const info = this.store.wizardInfo();
    const match = this.store.lookups().departments.find((o) => o.id === info.departmentId);
    return match ? this.localizedLabel(match.nameAr, match.nameEn) : '';
  }

  calculationMethodLabel(): string {
    return CALCULATION_METHOD_LABELS[this.store.wizardInfo().calculationMethod];
  }

  axisLabel(axisId: string): string {
    const axis = this.store.axisBankOptions().find((a) => a.id === axisId);
    return axis ? this.localizedLabel(axis.nameAr, axis.nameEn) : '';
  }

  availableAxisOptions(currentAxisId: string): SelectOption[] {
    const used = new Set(this.store.wizardAxes().map((a) => a.axisId));
    return this.store
      .axisBankOptions()
      .filter((a) => a.id === currentAxisId || !used.has(a.id))
      .map((a) => ({ label: this.localizedLabel(a.nameAr, a.nameEn), value: a.id }));
  }

  criterionBankOptions(axisId: string): SelectOption[] {
    return this.store.getCriterionBankOptions(axisId).map((c) => ({ label: this.localizedLabel(c.nameAr, c.nameEn), value: c.id }));
  }

  criterionDisplayName(axis: WizardAxisDraft, criterion: WizardCriterionDraft): string {
    if (criterion.mode === 'bank') {
      const match = this.store.getCriterionBankOptions(axis.axisId).find((c) => c.id === criterion.criterionId);
      return match ? this.localizedLabel(match.nameAr, match.nameEn) : '';
    }
    return this.localizedLabel(criterion.nameAr || '', criterion.nameEn);
  }

  onAxisSelected(axis: WizardAxisDraft, axisId: string) {
    this.service.updateAxisRow(axis.localId, { axisId });
  }

  onCriterionModeChange(axis: WizardAxisDraft, criterion: WizardCriterionDraft, mode: CriterionDraftMode) {
    this.service.updateCriterionRow(axis.localId, criterion.localId, {
      mode,
      criterionId: null,
      nameAr: '',
      nameEn: null,
      descriptionAr: null,
      descriptionEn: null,
    });
  }
}
