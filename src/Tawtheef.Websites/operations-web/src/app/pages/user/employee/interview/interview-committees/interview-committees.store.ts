import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';

import { TemplateModel } from '../interview-templates/models/template.model';
import { CommitteeModel } from './models/committee.model';
import { CommitteeMemberModel } from './models/committee-member.model';
import { CommitteeStatus, EvaluationScope } from './models/enums';
import {
  emptyWizardInfoDraft,
  newWizardMemberDraft,
  WizardInfoDraft,
  WizardJobContext,
  WizardMemberDraft,
  WizardMode,
  WizardTemplateContext,
  WizardUser,
} from './models/wizard-draft.model';

export type CommitteesView = 'list' | 'wizard' | 'detail';
export type WizardStep = 1 | 2 | 3;

// Chair plus two more members reaches the backend minimum, so a new wizard opens with that many blank rows.
const INITIAL_MEMBER_ROWS = 2;

@Injectable()
export class InterviewCommitteesStore {
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  view = signal<CommitteesView>('list');
  listLoading = signal(false);

  // ======== List ========
  private committeesResult = signal<CommitteeModel[]>([]);
  search = signal('');
  statusFilter = signal<CommitteeStatus | null>(null);

  committees = computed(() => {
    const term = this.search().trim().toLowerCase();
    const status = this.statusFilter();
    return this.committeesResult().filter((c) => {
      if (status !== null && c.status !== status) return false;
      if (!term) return true;
      return [c.code, c.nameAr, c.nameEn, c.jobTitleNameAr, c.jobTitleNameEn, c.chairNameAr, c.chairNameEn].some((value) =>
        (value ?? '').toLowerCase().includes(term),
      );
    });
  });

  // The backend allows one active committee per job, so these jobs are not offered when creating a new one.
  takenJobIds = computed(
    () => new Set(this.committeesResult().filter((c) => c.isActive).map((c) => c.jobId)),
  );

  // ======== Wizard lookups ========
  approvedTemplates = signal<TemplateModel[]>([]);

  // ======== Wizard ========
  wizardMode = signal<WizardMode>('create');
  wizardStep = signal<WizardStep>(1);
  wizardCommitteeId = signal<string | null>(null);
  wizardCommitteeCode = signal('');
  wizardInfo = signal<WizardInfoDraft>(emptyWizardInfoDraft());
  wizardJob = signal<WizardJobContext | null>(null);
  wizardTemplate = signal<WizardTemplateContext | null>(null);
  wizardTemplateLoading = signal(false);
  wizardChair = signal<WizardUser | null>(null);
  wizardMembers = signal<WizardMemberDraft[]>([]);
  wizardSaving = signal(false);

  // Everyone already on the roster - the pickers hide these so the same person can't be added twice.
  wizardSelectedUserIds = computed(() => {
    const ids = this.wizardMembers()
      .map((m) => m.user?.id)
      .filter((id): id is string => !!id);
    const chairId = this.wizardChair()?.id;
    if (chairId) ids.push(chairId);
    return new Set(ids);
  });

  // Chair counts toward the minimum, so the roster size is members + 1 once a chair is picked.
  wizardRosterSize = computed(() => this.wizardMembers().length + (this.wizardChair() ? 1 : 0));

  // ======== Detail ========
  detailCommittee = signal<CommitteeModel | null>(null);
  detailMembers = signal<CommitteeMemberModel[]>([]);
  detailLoading = signal(false);

  // ---- setters ----
  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setView(view: CommitteesView) {
    this.view.set(view);
  }

  setCommitteesResult(items: CommitteeModel[]) {
    this.committeesResult.set(items);
  }

  setSearch(value: string) {
    this.search.set(value ?? '');
  }

  setStatusFilter(value: CommitteeStatus | null) {
    this.statusFilter.set(value);
  }

  setApprovedTemplates(templates: TemplateModel[]) {
    this.approvedTemplates.set(templates);
  }

  // ---- Wizard mutations ----
  resetWizardForCreate() {
    this.wizardMode.set('create');
    this.wizardStep.set(1);
    this.wizardCommitteeId.set(null);
    this.wizardCommitteeCode.set('');
    this.wizardInfo.set(emptyWizardInfoDraft());
    this.wizardJob.set(null);
    this.wizardTemplate.set(null);
    this.wizardChair.set(null);
    this.wizardMembers.set(Array.from({ length: INITIAL_MEMBER_ROWS }, () => newWizardMemberDraft()));
    this.wizardSaving.set(false);
  }

  resetWizardForEdit(
    committee: CommitteeModel,
    job: WizardJobContext,
    template: WizardTemplateContext | null,
    chair: WizardUser | null,
    members: WizardMemberDraft[],
  ) {
    this.wizardMode.set('edit');
    this.wizardStep.set(1);
    this.wizardCommitteeId.set(committee.id);
    this.wizardCommitteeCode.set(committee.code);
    this.wizardInfo.set({
      jobId: committee.jobId,
      templateId: committee.interviewTemplateId,
      nameAr: committee.nameAr,
      nameEn: committee.nameEn ?? '',
      scopeDescription: committee.scopeDescription ?? null,
      notes: committee.notes ?? null,
    });
    this.wizardJob.set(job);
    this.wizardTemplate.set(template);
    this.wizardChair.set(chair);
    this.wizardMembers.set(members);
    this.wizardSaving.set(false);
  }

  // Once a committee has been created from the wizard, further saves must update it instead of creating a duplicate
  // (e.g. "Send for Approval" created it, then the submit call failed and the user retries).
  markWizardCreated(id: string) {
    this.wizardMode.set('edit');
    this.wizardCommitteeId.set(id);
  }

  setWizardStep(step: WizardStep) {
    this.wizardStep.set(step);
  }

  updateWizardInfo(patch: Partial<WizardInfoDraft>) {
    this.wizardInfo.update((v) => ({ ...v, ...patch }));
  }

  setWizardJob(job: WizardJobContext | null) {
    this.wizardJob.set(job);
  }

  setWizardTemplate(template: WizardTemplateContext | null) {
    this.wizardTemplate.set(template);
  }

  setWizardTemplateLoading(value: boolean) {
    this.wizardTemplateLoading.set(value);
  }

  setWizardChair(user: WizardUser | null) {
    this.wizardChair.set(user);
  }

  addWizardMember() {
    this.wizardMembers.update((list) => [...list, newWizardMemberDraft()]);
  }

  removeWizardMember(localId: string) {
    this.wizardMembers.update((list) => list.filter((m) => m.localId !== localId));
  }

  updateWizardMember(localId: string, patch: Partial<WizardMemberDraft>) {
    this.wizardMembers.update((list) => list.map((m) => (m.localId === localId ? { ...m, ...patch } : m)));
  }

  // Axes belong to one template's approved version, so picking a different template invalidates every selection.
  clearWizardMemberAxes() {
    this.wizardMembers.update((list) =>
      list.map((m) => (m.evaluationScope === EvaluationScope.SelectedAxes ? { ...m, axisIds: [] } : m)),
    );
  }

  setWizardSaving(value: boolean) {
    this.wizardSaving.set(value);
  }

  // ---- Detail ----
  setDetailCommittee(committee: CommitteeModel | null) {
    this.detailCommittee.set(committee);
  }

  setDetailMembers(members: CommitteeMemberModel[]) {
    this.detailMembers.set(members);
  }

  setDetailLoading(value: boolean) {
    this.detailLoading.set(value);
  }
}
