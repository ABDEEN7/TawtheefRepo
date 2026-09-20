import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';

import { InterviewTemplatesStore, WizardStep } from './interview-templates.store';
import { InterviewTemplatesService } from './services/interview-templates.service';
import { TemplateModel } from './models/template.model';
import { TemplateVersionModel } from './models/template-version.model';
import { CalculationMethod } from './models/enums';
import { WizardAxisDraft, WizardCriterionDraft, WizardInfoDraft } from './models/wizard-draft.model';

@Injectable()
export class InterviewTemplatesFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(InterviewTemplatesStore);
  private api = inject(InterviewTemplatesService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);

  init() {
    this.store.setCurrentLang(this.language.get());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((lang) => this.store.setCurrentLang(lang));

    this.loadTemplates();
    this.loadLookups();
    this.loadAxisBankOptions();
  }

  // ======== List ========
  loadTemplates() {
    this.store.listLoading.set(true);
    this.api.listTemplates().subscribe({
      next: (items) => {
        this.store.setTemplatesResult(items);
        this.store.listLoading.set(false);
      },
      error: () => this.store.listLoading.set(false),
    });
  }

  setSearch(value: string) {
    this.store.setSearch(value);
  }

  setStatusFilter(value: boolean | null) {
    this.store.setStatusFilter(value);
  }

  toggleTemplateActive(template: TemplateModel, isActive: boolean) {
    this.api.changeTemplateActivation(template.id, isActive).subscribe({
      next: () => {
        this.toast('INTERVIEW_TEMPLATES.STATUS_UPDATED');
        this.loadTemplates();
      },
    });
  }

  // ======== Lookups ========
  loadLookups() {
    this.api.getLookups().subscribe({
      next: (lookups) => this.store.setLookups(lookups),
    });
  }

  loadAxisBankOptions() {
    this.api.getAxisOptions().subscribe({
      next: (result) => this.store.setAxisBankOptions(result.items),
    });
  }

  loadCriterionBankOptions(axisId: string) {
    if (!axisId) return;
    this.api.getCriterionOptions(axisId).subscribe({
      next: (result) => this.store.setCriterionBankOptions(axisId, result.items),
    });
  }

  // ======== Wizard navigation ========
  openCreateWizard() {
    this.store.resetWizard('new-template');
    this.store.setView('wizard');
  }

  openCreateVersionWizard(template: TemplateModel) {
    this.store.resetWizard('new-version', template);
    this.store.setView('wizard');
  }

  openEditVersionWizard(template: TemplateModel, version: TemplateVersionModel) {
    this.api.getTemplateVersionDetails(version.id).subscribe({
      next: (details) => {
        this.store.resetWizard('edit-version', template, details);
        this.store.setView('wizard');
        for (const axis of details.axes) {
          this.loadCriterionBankOptions(axis.interviewEvaluationAxisId);
        }
      },
    });
  }

  cancelWizard() {
    this.store.setView('list');
    this.loadTemplates();
  }

  nextStep() {
    if (!this.validateStep(this.store.wizardStep())) return;
    const step = this.store.wizardStep();
    if (step < 4) this.store.setWizardStep((step + 1) as WizardStep);
  }

  prevStep() {
    const step = this.store.wizardStep();
    if (step > 1) this.store.setWizardStep((step - 1) as WizardStep);
  }

  goToStep(step: WizardStep) {
    const current = this.store.wizardStep();
    if (step === current) return;
    if (step < current) {
      this.store.setWizardStep(step);
      return;
    }
    for (let s = current; s < step; s++) {
      if (!this.validateStep(s as WizardStep)) return;
    }
    this.store.setWizardStep(step);
  }

  // ======== Wizard: step 1 ========
  updateInfo(patch: Partial<WizardInfoDraft>) {
    this.store.updateWizardInfo(patch);
  }

  // ======== Wizard: step 2 (axes) ========
  addAxisRow() {
    this.store.addWizardAxis();
  }

  removeAxisRow(localId: string) {
    this.store.removeWizardAxis(localId);
  }

  updateAxisRow(localId: string, patch: Partial<WizardAxisDraft>) {
    if (patch.axisId) {
      const current = this.store.wizardAxes().find((a) => a.localId === localId);
      const isReassignment = !!current && current.axisId !== patch.axisId;

      // Switching the bank axis invalidates any criteria already picked for the previous one.
      const nextPatch: Partial<WizardAxisDraft> = { ...patch, criteria: [] };

      // An existing (already-persisted) row's bank axis can't be changed via Update - the backend
      // command only carries scores/order, not a new InterviewEvaluationAxisId. Treat a reassignment
      // as remove-old + add-new: drop its backend id so submit adds it fresh under the new axis.
      if (isReassignment && current!.id) {
        this.store.markAxisRemoved(current!.id);
        nextPatch.id = null;
      }

      this.store.updateWizardAxis(localId, nextPatch);
      this.loadCriterionBankOptions(patch.axisId);
    } else {
      this.store.updateWizardAxis(localId, patch);
    }
  }

  // ======== Wizard: step 3 (criteria) ========
  addCriterionRow(axisLocalId: string) {
    this.store.addWizardCriterion(axisLocalId);
  }

  removeCriterionRow(axisLocalId: string, criterionLocalId: string) {
    this.store.removeWizardCriterion(axisLocalId, criterionLocalId);
  }

  updateCriterionRow(axisLocalId: string, criterionLocalId: string, patch: Partial<WizardCriterionDraft>) {
    this.store.updateWizardCriterion(axisLocalId, criterionLocalId, patch);
  }

  // ======== Validation ========
  private validateStep(step: WizardStep): boolean {
    if (step === 1) return this.validateStep1();
    if (step === 2) return this.validateStep2();
    if (step === 3) return this.validateStep3();
    return true;
  }

  private validateStep1(): boolean {
    const info = this.store.wizardInfo();
    if (!info.titleAr?.trim()) return this.fail('INTERVIEW_TEMPLATES.VALIDATION.TITLE_REQUIRED');
    if (!info.finalScore || info.finalScore < 1) return this.fail('INTERVIEW_TEMPLATES.VALIDATION.FINAL_SCORE_POSITIVE');
    if (
      info.qualificationScore !== null &&
      (info.qualificationScore < 0 || info.qualificationScore > info.finalScore)
    )
      return this.fail('INTERVIEW_TEMPLATES.VALIDATION.QUALIFICATION_SCORE_RANGE');
    return true;
  }

  private validateStep2(): boolean {
    const axes = this.store.wizardAxes();
    if (!axes.length) return this.fail('INTERVIEW_TEMPLATES.VALIDATION.AT_LEAST_ONE_AXIS');
    if (axes.some((a) => !a.axisId)) return this.fail('INTERVIEW_TEMPLATES.VALIDATION.AXIS_REQUIRED');
    if (new Set(axes.map((a) => a.axisId)).size !== axes.length)
      return this.fail('INTERVIEW_TEMPLATES.VALIDATION.AXIS_DUPLICATE');
    if (axes.some((a) => !a.maxScore || a.maxScore < 1))
      return this.fail('INTERVIEW_TEMPLATES.VALIDATION.AXIS_MAX_SCORE_POSITIVE');

    const total = this.store.wizardAxesTotal();
    const finalScore = this.store.wizardInfo().finalScore;
    if (total !== finalScore)
      return this.fail('INTERVIEW_TEMPLATES.VALIDATION.AXES_TOTAL_MISMATCH', { total, finalScore });

    return true;
  }

  private validateStep3(): boolean {
    const axes = this.store.wizardAxes();
    for (const axis of axes) {
      if (!axis.criteria.length) return this.fail('INTERVIEW_TEMPLATES.VALIDATION.AXIS_NEEDS_CRITERION');
      for (const criterion of axis.criteria) {
        if (criterion.mode === 'bank' && !criterion.criterionId)
          return this.fail('INTERVIEW_TEMPLATES.VALIDATION.CRITERION_BANK_REQUIRED');
        if (criterion.mode === 'custom' && !criterion.nameAr?.trim())
          return this.fail('INTERVIEW_TEMPLATES.VALIDATION.CRITERION_NAME_REQUIRED');
        if (!criterion.maxScore || criterion.maxScore < 1)
          return this.fail('INTERVIEW_TEMPLATES.VALIDATION.CRITERION_MAX_SCORE_POSITIVE');
      }
      const total = this.store.axisCriteriaTotal(axis);
      if (total !== axis.maxScore)
        return this.fail('INTERVIEW_TEMPLATES.VALIDATION.CRITERIA_TOTAL_MISMATCH', {
          total,
          axisMaxScore: axis.maxScore,
        });
    }
    return true;
  }

  private fail(key: string, params?: Record<string, unknown>): false {
    this.notify.error(this.translate.instant(key, params));
    return false;
  }

  // ======== Submit ========
  async submitWizard(): Promise<void> {
    if (!this.validateStep1() || !this.validateStep2() || !this.validateStep3()) return;

    this.store.setWizardSaving(true);
    try {
      const info = this.store.wizardInfo();
      const mode = this.store.wizardMode();

      let templateId = this.store.wizardTemplateId();
      if (mode === 'new-template') {
        templateId = await firstValueFrom(
          this.api.createTemplate({
            titleAr: info.titleAr.trim(),
            titleEn: info.titleEn?.trim() || null,
            organizationScopeId: info.organizationScopeId || null,
            jobTitleId: info.jobTitleId || null,
            departmentId: info.departmentId || null,
            isActive: true,
          })
        );
        this.store.setWizardTemplateId(templateId!);
      } else if (mode === 'edit-version') {
        if (!templateId) throw new Error('Missing template id');
        await firstValueFrom(
          this.api.updateTemplate({
            id: templateId,
            titleAr: info.titleAr.trim(),
            titleEn: info.titleEn?.trim() || null,
            organizationScopeId: info.organizationScopeId || null,
            jobTitleId: info.jobTitleId || null,
            departmentId: info.departmentId || null,
            isActive: this.store.wizardTemplateIsActive(),
          })
        );
      }
      if (!templateId) throw new Error('Missing template id');

      let versionId: string;
      if (mode === 'edit-version') {
        versionId = this.store.wizardVersionId()!;
        await firstValueFrom(
          this.api.updateTemplateVersion({
            id: versionId,
            finalScore: info.finalScore,
            qualificationScore: info.qualificationScore,
            calculationMethod: info.calculationMethod as CalculationMethod,
          })
        );
      } else {
        versionId = (await firstValueFrom(
          this.api.createTemplateVersion({
            interviewTemplateId: templateId,
            finalScore: info.finalScore,
            qualificationScore: info.qualificationScore,
            calculationMethod: info.calculationMethod as CalculationMethod,
          })
        ))!;
      }

      await this.applyAxesAndCriteria(versionId);
      await firstValueFrom(this.api.submitTemplateVersion(versionId));

      this.toast(
        mode === 'edit-version' ? 'INTERVIEW_TEMPLATES.RESUBMIT_SUCCESS' : 'INTERVIEW_TEMPLATES.SUBMIT_SUCCESS'
      );
      this.store.setView('list');
      this.loadTemplates();
    } catch {
      // error toast already shown by the global HTTP error interceptor
    } finally {
      this.store.setWizardSaving(false);
    }
  }

  // Applies the wizard's axes/criteria draft against the backend for the given version:
  // removes first (an axis removal cascades its own criteria server-side), then upserts every
  // remaining row - update when it carries an existing id, add when it was created in this session.
  private async applyAxesAndCriteria(versionId: string): Promise<void> {
    for (const axisId of this.store.wizardRemovedAxisIds()) {
      await firstValueFrom(this.api.removeTemplateVersionAxis(axisId));
    }
    for (const criterionId of this.store.wizardRemovedCriterionIds()) {
      await firstValueFrom(this.api.removeTemplateVersionCriterion(criterionId));
    }

    for (const axis of this.store.wizardAxes()) {
      let axisId: string;
      if (axis.id) {
        await firstValueFrom(
          this.api.updateTemplateVersionAxis({
            id: axis.id,
            maxScore: axis.maxScore,
            qualificationScore: axis.qualificationScore,
            orderNo: axis.orderNo,
          })
        );
        axisId = axis.id;
      } else {
        axisId = (await firstValueFrom(
          this.api.addTemplateVersionAxis({
            interviewTemplateVersionId: versionId,
            interviewEvaluationAxisId: axis.axisId,
            maxScore: axis.maxScore,
            qualificationScore: axis.qualificationScore,
            orderNo: axis.orderNo,
          })
        ))!;
      }

      for (const criterion of axis.criteria) {
        const payload = {
          interviewEvaluationCriterionId: criterion.mode === 'bank' ? criterion.criterionId : null,
          nameAr: criterion.mode === 'custom' ? criterion.nameAr.trim() : null,
          nameEn: criterion.mode === 'custom' ? criterion.nameEn?.trim() || null : null,
          descriptionAr: criterion.mode === 'custom' ? criterion.descriptionAr?.trim() || null : null,
          descriptionEn: criterion.mode === 'custom' ? criterion.descriptionEn?.trim() || null : null,
          maxScore: criterion.maxScore,
          isRequired: criterion.isRequired,
          orderNo: criterion.orderNo,
          notes: criterion.notes?.trim() || null,
        };

        if (criterion.id) {
          await firstValueFrom(this.api.updateTemplateVersionCriterion({ id: criterion.id, ...payload }));
        } else {
          await firstValueFrom(
            this.api.addTemplateVersionCriterion({ interviewTemplateEvaluationAxisId: axisId, ...payload })
          );
        }
      }
    }
  }

  // ======== Detail ========
  openDetail(template: TemplateModel) {
    this.store.setDetailTemplate(template);
    this.store.setView('detail');
    this.loadVersions(template.id);
  }

  backToList() {
    this.store.setView('list');
    this.loadTemplates();
  }

  loadVersions(templateId: string) {
    this.store.setDetailLoading(true);
    this.store.setDetailSelectedVersion(null);
    this.api.listTemplateVersions(templateId).subscribe({
      next: (versions) => {
        this.store.setDetailVersions(versions);
        this.store.setDetailLoading(false);
        if (versions.length) this.loadVersionDetails(versions[0].id);
      },
      error: () => this.store.setDetailLoading(false),
    });
  }

  loadVersionDetails(versionId: string) {
    this.api.getTemplateVersionDetails(versionId).subscribe({
      next: (details) => this.store.setDetailSelectedVersion(details),
    });
  }

  approveVersion(versionId: string, decisionNotes: string | null) {
    this.api.approveTemplateVersion(versionId, decisionNotes).subscribe({
      next: () => {
        this.toast('INTERVIEW_TEMPLATES.VERSION_APPROVED');
        const templateId = this.store.detailTemplate()?.id;
        if (templateId) this.loadVersions(templateId);
      },
    });
  }

  returnVersion(versionId: string, reason: string) {
    this.api.returnTemplateVersion(versionId, reason).subscribe({
      next: () => {
        this.toast('INTERVIEW_TEMPLATES.VERSION_RETURNED');
        const templateId = this.store.detailTemplate()?.id;
        if (templateId) this.loadVersions(templateId);
      },
    });
  }

  cancelVersion(versionId: string, reason: string) {
    this.api.cancelTemplateVersion(versionId, reason).subscribe({
      next: () => {
        this.toast('INTERVIEW_TEMPLATES.VERSION_CANCELLED');
        const templateId = this.store.detailTemplate()?.id;
        if (templateId) this.loadVersions(templateId);
      },
    });
  }

  private toast(key: string) {
    this.notify.success(this.translate.instant(key));
  }
}
