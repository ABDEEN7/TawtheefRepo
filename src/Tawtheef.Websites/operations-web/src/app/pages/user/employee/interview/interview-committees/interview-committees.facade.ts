import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { catchError, distinctUntilChanged, firstValueFrom, forkJoin, map, Observable, of, shareReplay, switchMap } from 'rxjs';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';
import { JobStatus } from '../../../../../core/enums/lookups.enum';

import { JobLookupService } from '../../job-management/services/job-lookup.service';
import { JobResponse } from '../../job-management/models/job-response-model';
import { InterviewTemplatesService } from '../interview-templates/services/interview-templates.service';
import { TemplateVersionStatus } from '../interview-templates/models/enums';

import { InterviewCommitteesStore, WizardStep } from './interview-committees.store';
import { CommitteeMemberPayload, InterviewCommitteesService } from './services/interview-committees.service';
import { CommitteeModel } from './models/committee.model';
import { EligibleMemberModel } from './models/committee-member.model';
import { CommitteeRole, CommitteeStatus, EvaluationScope, MINIMUM_COMMITTEE_MEMBERS } from './models/enums';
import { LazySelectOption } from './models/lazy-select-option.model';
import {
  memberDraftFromExisting,
  participatesInEvaluation,
  permissionsForRole,
  userFromEligible,
  userFromMember,
  WizardInfoDraft,
  WizardJobContext,
  WizardMemberDraft,
  WizardTemplateContext,
  WizardUser,
} from './models/wizard-draft.model';

// Search-as-you-type dropdowns show at most this many rows; the user narrows further by typing.
const MAX_JOB_OPTIONS = 30;

// Which committee's detail view is open lives in the URL (?committee=<id>) rather than only in memory: switching the language
// reloads the whole app, and without it the page would come back on the list instead of the same committee.
const COMMITTEE_QUERY_PARAM = 'committee';

@Injectable()
export class InterviewCommitteesFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(InterviewCommitteesStore);
  private api = inject(InterviewCommitteesService);
  private templatesApi = inject(InterviewTemplatesService);
  private jobLookups = inject(JobLookupService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Resolved once, on the first job search: the Published status id comes from the job-status lookup.
  private publishedJobStatusId$ = this.jobLookups.loadJobStatus().pipe(
    map(() => this.jobLookups.getStatusIdByEnum(JobStatus.Published)),
    shareReplay(1),
  );

  init() {
    this.store.setCurrentLang(this.language.get());
    this.language.current$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((lang) => this.store.setCurrentLang(lang));

    // Emits the current value right away, so a page loaded on ?committee=<id> opens that detail before its first render.
    this.route.queryParamMap
      .pipe(
        map((params) => params.get(COMMITTEE_QUERY_PARAM)),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((id) => this.syncViewWithUrl(id));

    this.loadCommittees();
    this.loadApprovedTemplates();
  }

  // The URL decides between list and detail; the wizard is not URL-driven, so it is left alone here.
  private syncViewWithUrl(id: string | null) {
    if (id) {
      if (this.store.view() === 'detail' && this.store.detailCommittee()?.id === id) return;
      this.store.setView('detail');
      this.loadDetail(id);
      return;
    }

    if (this.store.view() === 'detail') {
      this.store.setView('list');
      this.loadCommittees();
    }
  }

  private setUrlCommittee(id: string | null) {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { [COMMITTEE_QUERY_PARAM]: id },
      queryParamsHandling: 'merge',
    });
  }

  // ======== List ========
  loadCommittees() {
    this.store.listLoading.set(true);
    this.api.listCommittees().subscribe({
      next: (items) => {
        this.store.setCommitteesResult(items);
        this.store.listLoading.set(false);
      },
      error: () => this.store.listLoading.set(false),
    });
  }

  setSearch(value: string) {
    this.store.setSearch(value);
  }

  setStatusFilter(value: CommitteeStatus | null) {
    this.store.setStatusFilter(value);
  }

  // ======== Lookups ========
  // A committee is built on a template's Approved version, and the list only exposes the latest version's status,
  // so a template whose newest version is still a draft is not offered until that version is approved.
  loadApprovedTemplates() {
    this.templatesApi.listTemplates().subscribe({
      next: (templates) =>
        this.store.setApprovedTemplates(
          templates.filter((t) => t.isActive && t.latestVersionStatus === TemplateVersionStatus.Approved),
        ),
    });
  }

  searchJobs = (term: string): Observable<LazySelectOption<WizardJobContext>[]> =>
    this.publishedJobStatusId$.pipe(
      switchMap((statusId) =>
        // Without the Published id the search would return every job, so return nothing instead.
        statusId ? this.api.searchPublishedJobs(statusId, term.trim()) : of(null),
      ),
      map((result) => {
        const taken = this.store.takenJobIds();
        return (result?.items ?? [])
          .filter((job) => !taken.has(job.id))
          .slice(0, MAX_JOB_OPTIONS)
          .map((job) => this.toJobOption(job));
      }),
      catchError(() => of([])),
    );

  searchEligibleMembers = (term: string): Observable<LazySelectOption<WizardUser>[]> =>
    this.api.searchEligibleMembers(term.trim()).pipe(
      map((members) => members.map((member) => this.toUserOption(member))),
      catchError(() => of([])),
    );

  private toJobOption(job: JobResponse): LazySelectOption<WizardJobContext> {
    const context: WizardJobContext = {
      id: job.id,
      titleAr: job.titleAr ?? '',
      titleEn: job.titleEn ?? null,
      jobNumber: job.jobNumber ?? null,
      departmentName: job.department?.name ?? job.management?.name ?? null,
      categoryName: job.jobCategory?.name ?? null,
    };
    return {
      value: job.id,
      label: this.localized(context.titleAr, context.titleEn),
      hint: context.jobNumber,
      searchText: [context.titleAr, context.titleEn, context.jobNumber].filter(Boolean).join(' '),
      data: context,
    };
  }

  private toUserOption(member: EligibleMemberModel): LazySelectOption<WizardUser> {
    const user = userFromEligible(member);
    return {
      value: user.id,
      label: this.userLabel(user),
      hint: user.email,
      searchText: [user.nameAr, user.nameEn, user.email].filter(Boolean).join(' '),
      data: user,
    };
  }

  userLabel(user: WizardUser): string {
    return this.localized(user.nameAr, user.nameEn);
  }

  private localized(ar: string, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  // ======== Wizard navigation ========
  openCreateWizard() {
    this.store.resetWizardForCreate();
    this.store.setView('wizard');
    this.loadApprovedTemplates();
  }

  openEditWizard(committee: CommitteeModel) {
    forkJoin({
      members: this.api.listMembers(committee.id),
      template: this.loadTemplateContext(committee.interviewTemplateId, committee.interviewTemplateTitleAr, committee.interviewTemplateTitleEn ?? null),
    }).subscribe({
      next: ({ members, template }) => {
        const assignableAxisIds = new Set(template?.axes.map((a) => a.id) ?? []);
        const chair = members.find((m) => m.role === CommitteeRole.Chair);
        const others = members
          .filter((m) => m.role !== CommitteeRole.Chair)
          .map((m) => memberDraftFromExisting(m, assignableAxisIds));

        const job: WizardJobContext = {
          id: committee.jobId,
          titleAr: committee.jobTitleNameAr ?? '',
          titleEn: committee.jobTitleNameEn ?? null,
          jobNumber: null,
          departmentName: null,
          categoryName: this.localized(committee.committeeTypeNameAr, committee.committeeTypeNameEn),
        };

        this.store.resetWizardForEdit(committee, job, template, chair ? userFromMember(chair) : null, others);
        this.store.setView('wizard');
        // Editing from the detail view leaves ?committee=<id> behind; clear it now that the wizard is showing (clearing it
        // earlier would make the URL sync send the user back to the list).
        this.setUrlCommittee(null);
      },
    });
  }

  cancelWizard() {
    this.store.setView('list');
    this.loadCommittees();
  }

  nextStep() {
    if (!this.validateStep(this.store.wizardStep())) return;
    const step = this.store.wizardStep();
    if (step < 3) this.store.setWizardStep((step + 1) as WizardStep);
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

  selectJob(option: LazySelectOption<WizardJobContext> | null) {
    this.store.updateWizardInfo({ jobId: option?.value ?? '' });
    this.store.setWizardJob(option?.data ?? null);
  }

  selectTemplate(templateId: string | null) {
    this.store.updateWizardInfo({ templateId: templateId ?? '' });
    this.store.clearWizardMemberAxes();
    this.store.setWizardTemplate(null);
    if (!templateId) return;

    const template = this.store.approvedTemplates().find((t) => t.id === templateId);
    this.loadTemplateContext(templateId, template?.titleAr ?? '', template?.titleEn ?? null).subscribe({
      next: (context) => {
        // The user may have picked something else while this was loading.
        if (this.store.wizardInfo().templateId === templateId) this.store.setWizardTemplate(context);
      },
    });
  }

  // Resolves the template's currently Approved version and its axes - the same rubric the backend validates the
  // per-member axis ids against.
  private loadTemplateContext(
    templateId: string,
    titleAr: string,
    titleEn: string | null,
  ): Observable<WizardTemplateContext | null> {
    this.store.setWizardTemplateLoading(true);
    return this.templatesApi.listTemplateVersions(templateId).pipe(
      switchMap((versions) => {
        const approved = versions.find((v) => v.status === TemplateVersionStatus.Approved);
        if (!approved) {
          this.fail('INTERVIEW_COMMITTEES.VALIDATION.TEMPLATE_NO_APPROVED_VERSION');
          return of(null);
        }
        return this.templatesApi.getTemplateVersionDetails(approved.id).pipe(
          map(
            (details): WizardTemplateContext => ({
              id: templateId,
              titleAr,
              titleEn,
              versionNo: details.versionNo,
              finalScore: details.finalScore,
              qualificationScore: details.qualificationScore ?? null,
              axes: details.axes.map((a) => ({ id: a.id, nameAr: a.axisNameAr, nameEn: a.axisNameEn ?? null })),
            }),
          ),
        );
      }),
      catchError(() => of(null)),
      map((context) => {
        this.store.setWizardTemplateLoading(false);
        return context;
      }),
    );
  }

  // ======== Wizard: step 2 ========
  setChair(user: WizardUser | null) {
    this.store.setWizardChair(user);
  }

  addMember() {
    this.store.addWizardMember();
  }

  removeMember(localId: string) {
    this.store.removeWizardMember(localId);
  }

  updateMember(localId: string, patch: Partial<WizardMemberDraft>) {
    this.store.updateWizardMember(localId, patch);
  }

  changeMemberRole(localId: string, role: CommitteeRole) {
    // Roles that don't evaluate have no evaluation scope, so drop any axis selection made under the previous role.
    const patch: Partial<WizardMemberDraft> = participatesInEvaluation(role)
      ? { role }
      : { role, evaluationScope: EvaluationScope.AllAxes, axisIds: [] };
    this.store.updateWizardMember(localId, patch);
  }

  changeMemberScope(localId: string, evaluationScope: EvaluationScope) {
    this.store.updateWizardMember(localId, { evaluationScope, axisIds: [] });
  }

  toggleMemberAxis(member: WizardMemberDraft, axisId: string, checked: boolean) {
    const axisIds = checked
      ? [...member.axisIds.filter((id) => id !== axisId), axisId]
      : member.axisIds.filter((id) => id !== axisId);
    this.store.updateWizardMember(member.localId, { axisIds });
  }

  // ======== Validation ========
  private validateStep(step: WizardStep): boolean {
    if (step === 1) return this.validateStep1();
    if (step === 2) return this.validateStep2();
    return true;
  }

  private validateStep1(): boolean {
    const info = this.store.wizardInfo();
    if (!info.jobId) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.JOB_REQUIRED');
    if (!info.templateId) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.TEMPLATE_REQUIRED');
    if (!info.nameAr?.trim()) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.NAME_REQUIRED');
    if (this.store.wizardTemplateLoading()) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.TEMPLATE_LOADING');
    if (!this.store.wizardTemplate()) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.TEMPLATE_NO_APPROVED_VERSION');
    return true;
  }

  private validateStep2(): boolean {
    const chair = this.store.wizardChair();
    const members = this.store.wizardMembers();

    if (!chair) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.CHAIR_REQUIRED');
    if (members.some((m) => !m.user)) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.MEMBER_USER_REQUIRED');

    const userIds = [chair.id, ...members.map((m) => m.user!.id)];
    if (new Set(userIds).size !== userIds.length) return this.fail('INTERVIEW_COMMITTEES.VALIDATION.MEMBER_DUPLICATE');

    if (userIds.length < MINIMUM_COMMITTEE_MEMBERS)
      return this.fail('INTERVIEW_COMMITTEES.VALIDATION.MINIMUM_MEMBERS', { count: MINIMUM_COMMITTEE_MEMBERS });

    const missingAxes = members.find(
      (m) => participatesInEvaluation(m.role) && m.evaluationScope === EvaluationScope.SelectedAxes && !m.axisIds.length,
    );
    if (missingAxes)
      return this.fail('INTERVIEW_COMMITTEES.VALIDATION.MEMBER_AXES_REQUIRED', { name: this.userLabel(missingAxes.user!) });

    return true;
  }

  private fail(key: string, params?: Record<string, unknown>): false {
    this.notify.error(this.translate.instant(key, params));
    return false;
  }

  // ======== Save / Submit ========
  // The backend rejects a committee with fewer than the minimum members even as a draft, so both actions run the same
  // full validation - "Save as Draft" only skips the approval hand-off, not the checks.
  async save(sendForApproval: boolean): Promise<void> {
    if (!this.validateStep1() || !this.validateStep2()) return;

    this.store.setWizardSaving(true);
    try {
      const info = this.store.wizardInfo();
      const members = this.buildMemberPayloads();
      const core = {
        nameAr: info.nameAr.trim(),
        nameEn: info.nameEn?.trim() || null,
        scopeDescription: info.scopeDescription,
        notes: info.notes,
        members,
      };

      let committeeId = this.store.wizardCommitteeId();
      if (this.store.wizardMode() === 'create') {
        committeeId = await firstValueFrom(
          this.api.createCommittee({ ...core, jobId: info.jobId, interviewTemplateId: info.templateId }),
        );
        // From here a retry must update this committee, not create a second one for the same job.
        this.store.markWizardCreated(committeeId);
      } else {
        await firstValueFrom(this.api.updateCommittee({ ...core, id: committeeId! }));
      }

      if (sendForApproval) await firstValueFrom(this.api.submitCommittee(committeeId!));

      this.toast(sendForApproval ? 'INTERVIEW_COMMITTEES.SUBMIT_SUCCESS' : 'INTERVIEW_COMMITTEES.DRAFT_SAVED');
      this.store.setView('list');
      this.loadCommittees();
    } catch {
      // error toast already shown by the global HTTP error interceptor
    } finally {
      this.store.setWizardSaving(false);
    }
  }

  // Chair evaluates every axis by definition (empty axis list = all axes); every other row sends only what it picked.
  private buildMemberPayloads(): CommitteeMemberPayload[] {
    const chair = this.store.wizardChair()!;
    const payloads = [this.toPayload(chair.id, CommitteeRole.Chair, [])];

    for (const member of this.store.wizardMembers()) {
      const scopedAxes =
        participatesInEvaluation(member.role) && member.evaluationScope === EvaluationScope.SelectedAxes
          ? member.axisIds
          : [];
      payloads.push(this.toPayload(member.user!.id, member.role, scopedAxes));
    }
    return payloads;
  }

  private toPayload(memberUserId: string, role: CommitteeRole, evaluationAxisIds: string[]): CommitteeMemberPayload {
    return {
      memberUserId,
      role,
      participatesInEvaluation: participatesInEvaluation(role),
      ...permissionsForRole(role),
      evaluationAxisIds,
    };
  }

  // ======== Detail ========
  openDetail(committee: CommitteeModel) {
    // Show what the list row already knows straight away; the URL change is what actually opens the view and loads
    // the fresh record (see syncViewWithUrl).
    this.store.setDetailCommittee(committee);
    this.store.setDetailMembers([]);
    this.setUrlCommittee(committee.id);
  }

  backToList() {
    this.setUrlCommittee(null);
  }

  loadDetail(id: string) {
    this.store.setDetailLoading(true);
    forkJoin({ committee: this.api.getCommittee(id), members: this.api.listMembers(id) }).subscribe({
      next: ({ committee, members }) => {
        this.store.setDetailCommittee(committee);
        this.store.setDetailMembers(members);
        this.store.setDetailLoading(false);
      },
      // A committee that can't be loaded (deleted, or a stale/bad ?committee= link) leaves nothing to show - back to the list.
      error: () => {
        this.store.setDetailLoading(false);
        this.backToList();
      },
    });
  }

  submitCommittee(id: string) {
    this.runDetailAction(this.api.submitCommittee(id), 'INTERVIEW_COMMITTEES.COMMITTEE_SUBMITTED', id);
  }

  approveCommittee(id: string, decisionNotes: string | null) {
    this.runDetailAction(this.api.approveCommittee(id, decisionNotes), 'INTERVIEW_COMMITTEES.COMMITTEE_APPROVED', id);
  }

  returnCommittee(id: string, reason: string) {
    this.runDetailAction(this.api.returnCommittee(id, reason), 'INTERVIEW_COMMITTEES.COMMITTEE_RETURNED', id);
  }

  cancelCommittee(id: string, reason: string) {
    this.runDetailAction(this.api.cancelCommittee(id, reason), 'INTERVIEW_COMMITTEES.COMMITTEE_CANCELLED', id);
  }

  private runDetailAction(action$: Observable<void>, successKey: string, id: string) {
    action$.subscribe({
      next: () => {
        this.toast(successKey);
        this.loadDetail(id);
      },
    });
  }

  private toast(key: string) {
    this.notify.success(this.translate.instant(key));
  }
}
