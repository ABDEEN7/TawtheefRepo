import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';

import { TemplateModel } from './models/template.model';
import { TemplateVersionDetailsModel, TemplateVersionModel } from './models/template-version.model';
import { TemplateLookupsModel } from './models/lookup-option.model';
import { AxisBankOption, CriterionBankOption } from './models/bank-option.model';
import {
  axisDraftFromExisting,
  emptyWizardInfoDraft,
  newWizardAxisDraft,
  newWizardCriterionDraft,
  WizardAxisDraft,
  WizardCriterionDraft,
  WizardInfoDraft,
  WizardMode,
} from './models/wizard-draft.model';

export type TemplatesView = 'list' | 'wizard' | 'detail';
export type WizardStep = 1 | 2 | 3 | 4;

@Injectable()
export class InterviewTemplatesStore {
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  view = signal<TemplatesView>('list');
  listLoading = signal(false);

  // ======== List ========
  private templatesResult = signal<TemplateModel[]>([]);
  search = signal('');
  statusFilter = signal<boolean | null>(null);

  templates = computed(() => {
    const term = this.search().trim().toLowerCase();
    const status = this.statusFilter();
    return this.templatesResult().filter((t) => {
      if (status !== null && t.isActive !== status) return false;
      if (!term) return true;
      return (
        t.titleAr.toLowerCase().includes(term) ||
        (t.titleEn ?? '').toLowerCase().includes(term) ||
        (t.jobTitleNameAr ?? '').toLowerCase().includes(term) ||
        (t.jobTitleNameEn ?? '').toLowerCase().includes(term) ||
        (t.departmentNameAr ?? '').toLowerCase().includes(term) ||
        (t.departmentNameEn ?? '').toLowerCase().includes(term) ||
        (t.organizationScopeNameAr ?? '').toLocaleLowerCase().includes(term) ||
        (t.organizationScopeNameEn ?? '').toLocaleLowerCase().includes(term)
      );
    });
  });

  // ======== Lookups ========
  lookups = signal<TemplateLookupsModel>({
    organizationScopes: [],
    jobTitles: [],
    departments: [],
  });
  axisBankOptions = signal<AxisBankOption[]>([]);
  criterionBankByAxis = signal<Record<string, CriterionBankOption[]>>({});

  // ======== Wizard ========
  wizardMode = signal<WizardMode>('new-template');
  wizardStep = signal<WizardStep>(1);
  wizardTemplateId = signal<string | null>(null);
  wizardTemplateTitle = signal<string>('');
  wizardTemplateIsActive = signal<boolean>(true);
  wizardVersionId = signal<string | null>(null);
  wizardInfo = signal<WizardInfoDraft>(emptyWizardInfoDraft());
  wizardAxes = signal<WizardAxisDraft[]>([]);
  wizardSaving = signal(false);

  // Existing backend ids removed while editing a version - applied as Remove calls on submit.
  // An axis removal cascades its criteria server-side, so a whole-axis removal is never also
  // recorded here for its child criteria (see removeWizardAxis).
  wizardRemovedAxisIds = signal<string[]>([]);
  wizardRemovedCriterionIds = signal<string[]>([]);

  wizardAxesTotal = computed(() =>
    this.wizardAxes().reduce((sum, a) => sum + (+a.maxScore || 0), 0),
  );

  axisCriteriaTotal(axis: WizardAxisDraft): number {
    return axis.criteria.reduce((sum, c) => sum + (+c.maxScore || 0), 0);
  }

  // ======== Detail ========
  detailTemplate = signal<TemplateModel | null>(null);
  detailVersions = signal<TemplateVersionModel[]>([]);
  detailSelectedVersion = signal<TemplateVersionDetailsModel | null>(null);
  detailLoading = signal(false);

  // ---- setters ----
  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setView(view: TemplatesView) {
    this.view.set(view);
  }

  setTemplatesResult(items: TemplateModel[]) {
    this.templatesResult.set(items);
  }

  setSearch(value: string) {
    this.search.set(value ?? '');
  }

  setStatusFilter(value: boolean | null) {
    this.statusFilter.set(value);
  }

  setLookups(lookups: TemplateLookupsModel) {
    this.lookups.set(lookups);
  }

  setAxisBankOptions(options: AxisBankOption[]) {
    this.axisBankOptions.set(options);
  }

  setCriterionBankOptions(axisId: string, options: CriterionBankOption[]) {
    this.criterionBankByAxis.update((map) => ({ ...map, [axisId]: options }));
  }

  getCriterionBankOptions(axisId: string): CriterionBankOption[] {
    return this.criterionBankByAxis()[axisId] ?? [];
  }

  // ---- Wizard mutations ----
  resetWizard(
    mode: WizardMode,
    template?: TemplateModel,
    versionDetails?: TemplateVersionDetailsModel,
  ) {
    this.wizardMode.set(mode);
    this.wizardStep.set(1);
    this.wizardTemplateId.set(mode !== 'new-template' ? (template?.id ?? null) : null);
    this.wizardTemplateTitle.set(template?.titleAr ?? '');
    this.wizardTemplateIsActive.set(template?.isActive ?? true);
    this.wizardVersionId.set(mode === 'edit-version' ? (versionDetails?.id ?? null) : null);

    if (mode === 'edit-version' && template && versionDetails) {
      this.wizardInfo.set({
        titleAr: template.titleAr,
        titleEn: template.titleEn ?? '',
        organizationScopeId: template.organizationScopeId ?? '',
        jobTitleId: template.jobTitleId ?? '',
        departmentId: template.departmentId ?? '',
        finalScore: versionDetails.finalScore,
        qualificationScore: versionDetails.qualificationScore ?? null,
        calculationMethod: versionDetails.calculationMethod,
      });
      this.wizardAxes.set(versionDetails.axes.map(axisDraftFromExisting));
    } else if (mode === 'new-version' && template) {
      this.wizardInfo.set({
        ...emptyWizardInfoDraft(),
        titleAr: template.titleAr,
        titleEn: template.titleEn ?? '',
        organizationScopeId: template.organizationScopeId ?? '',
        jobTitleId: template.jobTitleId ?? '',
        departmentId: template.departmentId ?? '',
      });
      this.wizardAxes.set([]);
    } else {
      this.wizardInfo.set(emptyWizardInfoDraft());
      this.wizardAxes.set([]);
    }

    this.wizardRemovedAxisIds.set([]);
    this.wizardRemovedCriterionIds.set([]);
    this.wizardSaving.set(false);
  }

  setWizardStep(step: WizardStep) {
    this.wizardStep.set(step);
  }

  setWizardTemplateId(id: string) {
    this.wizardTemplateId.set(id);
  }

  updateWizardInfo(patch: Partial<WizardInfoDraft>) {
    this.wizardInfo.update((v) => ({ ...v, ...patch }));
  }

  addWizardAxis() {
    const used = new Set(this.wizardAxes().map((a) => a.axisId));
    const available = this.axisBankOptions().find((a) => !used.has(a.id));
    const draft = newWizardAxisDraft(available?.id ?? '');
    draft.orderNo = this.wizardAxes().length + 1;
    this.wizardAxes.update((list) => [...list, draft]);
  }

  // Records an existing backend axis id as removed - used both for an outright row removal and
  // for reassigning an existing row to a different bank axis (see markAxisReassigned below).
  markAxisRemoved(id: string) {
    this.wizardRemovedAxisIds.update((ids) => [...ids, id]);
  }

  removeWizardAxis(localId: string) {
    const removed = this.wizardAxes().find((a) => a.localId === localId);
    if (removed?.id) {
      this.markAxisRemoved(removed.id);
    }
    this.wizardAxes.update((list) =>
      list.filter((a) => a.localId !== localId).map((a, i) => ({ ...a, orderNo: i + 1 })),
    );
  }

  updateWizardAxis(localId: string, patch: Partial<WizardAxisDraft>) {
    this.wizardAxes.update((list) =>
      list.map((a) => (a.localId === localId ? { ...a, ...patch } : a)),
    );
  }

  addWizardCriterion(axisLocalId: string) {
    this.wizardAxes.update((list) =>
      list.map((a) => {
        if (a.localId !== axisLocalId) return a;
        const draft = newWizardCriterionDraft();
        draft.orderNo = a.criteria.length + 1;
        return { ...a, criteria: [...a.criteria, draft] };
      }),
    );
  }

  removeWizardCriterion(axisLocalId: string, criterionLocalId: string) {
    const axis = this.wizardAxes().find((a) => a.localId === axisLocalId);
    const removed = axis?.criteria.find((c) => c.localId === criterionLocalId);
    if (removed?.id) {
      this.wizardRemovedCriterionIds.update((ids) => [...ids, removed.id!]);
    }
    this.wizardAxes.update((list) =>
      list.map((a) => {
        if (a.localId !== axisLocalId) return a;
        const criteria = a.criteria
          .filter((c) => c.localId !== criterionLocalId)
          .map((c, i) => ({ ...c, orderNo: i + 1 }));
        return { ...a, criteria };
      }),
    );
  }

  updateWizardCriterion(
    axisLocalId: string,
    criterionLocalId: string,
    patch: Partial<WizardCriterionDraft>,
  ) {
    this.wizardAxes.update((list) =>
      list.map((a) => {
        if (a.localId !== axisLocalId) return a;
        return {
          ...a,
          criteria: a.criteria.map((c) =>
            c.localId === criterionLocalId ? { ...c, ...patch } : c,
          ),
        };
      }),
    );
  }

  setWizardSaving(value: boolean) {
    this.wizardSaving.set(value);
  }

  // ---- Detail ----
  setDetailTemplate(template: TemplateModel | null) {
    this.detailTemplate.set(template);
  }

  setDetailVersions(versions: TemplateVersionModel[]) {
    this.detailVersions.set(versions);
  }

  setDetailSelectedVersion(version: TemplateVersionDetailsModel | null) {
    this.detailSelectedVersion.set(version);
  }

  setDetailLoading(value: boolean) {
    this.detailLoading.set(value);
  }
}
