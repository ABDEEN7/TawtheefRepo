import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';

import { AppointmentModel } from './models/appointment.model';
import { InterviewType, ScheduleStatus } from './models/enums';
import { SchedulePlanPreviewModel } from './models/plan-preview.model';
import { CreationContextModel, ScheduleListItemModel, ScheduleModel } from './models/schedule.model';
import {
  emptyWizardInfoDraft,
  ManualAssignmentDraft,
  manualAssignmentsFromAppointments,
  PeriodDraft,
  periodDraftFromSchedule,
  WizardInfoDraft,
  WizardJobContext,
  WizardMode,
} from './models/wizard-draft.model';

export type SchedulesView = 'list' | 'wizard' | 'detail';
export type WizardStep = 1 | 2 | 3 | 4;

@Injectable()
export class InterviewScheduleStore {
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  view = signal<SchedulesView>('list');
  listLoading = signal(false);

  // ======== List ========
  private schedulesResult = signal<ScheduleListItemModel[]>([]);
  search = signal('');
  statusFilter = signal<ScheduleStatus | null>(null);
  // Holds a job *title* id: the Job column shows titles, and jobs sharing a title are indistinguishable there.
  jobFilter = signal<string | null>(null);
  typeFilter = signal<InterviewType | null>(null);

  // The Job filter offers exactly the jobs that have a schedule (one entry per title), so there is no separate
  // lookup to fetch or page - it is derived from the rows already loaded.
  jobOptions = computed(() => {
    const options = new Map<string, { value: string; label: string }>();
    for (const s of this.schedulesResult()) {
      if (!s.jobTitleId || options.has(s.jobTitleId)) continue;
      const label = this.isRtl() ? s.jobTitleNameAr || s.jobTitleNameEn : s.jobTitleNameEn || s.jobTitleNameAr;
      if (label) options.set(s.jobTitleId, { value: s.jobTitleId, label });
    }
    return [...options.values()].sort((a, b) => a.label.localeCompare(b.label));
  });

  // Paging is over the filtered rows (the list endpoint returns every schedule and the filters run client-side), so
  // the table only ever renders one page while the pagination bar counts all matches.
  page = signal(1);
  pageSize = signal(20);

  schedules = computed(() => {
    const term = this.search().trim().toLowerCase();
    const status = this.statusFilter();
    const jobTitleId = this.jobFilter();
    const type = this.typeFilter();
    return this.schedulesResult().filter((s) => {
      if (status !== null && s.status !== status) return false;
      if (jobTitleId !== null && s.jobTitleId !== jobTitleId) return false;
      if (type !== null && s.defaultInterviewType !== type) return false;
      if (!term) return true;
      return [s.jobTitleNameAr, s.jobTitleNameEn].some((value) => (value ?? '').toLowerCase().includes(term));
    });
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.schedules().length / this.pageSize())));

  // The requested page is clamped, so a refresh or filter that leaves fewer rows never strands the user on a page that
  // no longer exists.
  currentPage = computed(() => Math.min(this.page(), this.totalPages()));

  pagedSchedules = computed(() => {
    const size = this.pageSize();
    const start = (this.currentPage() - 1) * size;
    return this.schedules().slice(start, start + size);
  });

  // ======== Wizard ========
  wizardMode = signal<WizardMode>('create');
  wizardStep = signal<WizardStep>(1);
  wizardScheduleId = signal<string | null>(null);
  wizardInfo = signal<WizardInfoDraft>(emptyWizardInfoDraft());
  wizardJob = signal<WizardJobContext | null>(null);

  // Step 1/2 job context (job identity, committee, eligible-candidate stats) - null until a job is picked.
  wizardContext = signal<CreationContextModel | null>(null);
  wizardContextLoading = signal(false);
  // Set when the job has no approved committee/template - blocks moving past step 1 (see facade.validateStep1).
  wizardContextError = signal<string | null>(null);

  wizardDuration = signal(30);
  wizardBuffer = signal(10);
  wizardPeriods = signal<PeriodDraft[]>([]);
  wizardManualAssignments = signal<ManualAssignmentDraft[]>([]);

  // Server-computed capacity/distribution for steps 2-4 - never derived client-side (see PreviewScheduleSlotsQuery).
  wizardPreview = signal<SchedulePlanPreviewModel | null>(null);
  wizardPreviewLoading = signal(false);

  wizardSaving = signal(false);

  // ======== Detail ========
  detailSchedule = signal<ScheduleModel | null>(null);
  detailAppointments = signal<AppointmentModel[]>([]);
  detailLoading = signal(false);

  // ---- setters ----
  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setView(view: SchedulesView) {
    this.view.set(view);
  }

  setSchedulesResult(items: ScheduleListItemModel[]) {
    this.schedulesResult.set(items);
  }

  // Any filter change starts over from the first page.
  setSearch(value: string) {
    this.search.set(value ?? '');
    this.page.set(1);
  }

  setStatusFilter(value: ScheduleStatus | null) {
    this.statusFilter.set(value);
    this.page.set(1);
  }

  setJobFilter(value: string | null) {
    this.jobFilter.set(value);
    this.page.set(1);
  }

  setTypeFilter(value: InterviewType | null) {
    this.typeFilter.set(value);
    this.page.set(1);
  }

  setPage(page: number) {
    this.page.set(page);
  }

  setPageSize(size: number) {
    this.pageSize.set(size);
    this.page.set(1);
  }

  // ---- Wizard mutations ----
  resetWizardForCreate() {
    this.wizardMode.set('create');
    this.wizardStep.set(1);
    this.wizardScheduleId.set(null);
    this.wizardInfo.set(emptyWizardInfoDraft());
    this.wizardJob.set(null);
    this.wizardContext.set(null);
    this.wizardContextError.set(null);
    this.wizardDuration.set(30);
    this.wizardBuffer.set(10);
    this.wizardPeriods.set([]);
    this.wizardManualAssignments.set([]);
    this.wizardPreview.set(null);
    this.wizardSaving.set(false);
  }

  // Restores everything that was saved: step 1's job/type/context, the duration/buffer, the periods the slots came
  // from, and the candidate distribution (as pins, so the server reproduces it - see manualAssignmentsFromAppointments).
  // The preview (capacity + slot table) is fetched by the facade right after.
  resetWizardForEdit(schedule: ScheduleModel, job: WizardJobContext, context: CreationContextModel) {
    this.wizardMode.set('edit');
    this.wizardStep.set(1);
    this.wizardScheduleId.set(schedule.id);
    this.wizardInfo.set({ jobId: schedule.jobId, interviewType: schedule.defaultInterviewType });
    this.wizardJob.set(job);
    this.wizardContext.set(context);
    this.wizardContextError.set(null);
    this.wizardDuration.set(schedule.defaultDurationMinutes);
    this.wizardBuffer.set(schedule.defaultBufferMinutes);
    this.wizardPeriods.set(schedule.periods.map(periodDraftFromSchedule));
    this.wizardManualAssignments.set(manualAssignmentsFromAppointments(schedule.appointments));
    this.wizardPreview.set(null);
    this.wizardSaving.set(false);
  }

  // Once a schedule has been created from the wizard, further saves must update it instead of creating a
  // duplicate (e.g. "Save as Draft" created it, then a later save in the same session retries).
  markWizardCreated(id: string) {
    this.wizardMode.set('edit');
    this.wizardScheduleId.set(id);
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

  setWizardContext(context: CreationContextModel | null) {
    this.wizardContext.set(context);
  }

  setWizardContextLoading(value: boolean) {
    this.wizardContextLoading.set(value);
  }

  setWizardContextError(key: string | null) {
    this.wizardContextError.set(key);
  }

  setWizardDuration(value: number) {
    this.wizardDuration.set(value);
  }

  setWizardBuffer(value: number) {
    this.wizardBuffer.set(value);
  }

  addWizardPeriod(period: PeriodDraft) {
    this.wizardPeriods.update((list) => [...list, period]);
  }

  updateWizardPeriod(localId: string, period: PeriodDraft) {
    this.wizardPeriods.update((list) => list.map((p) => (p.localId === localId ? period : p)));
  }

  removeWizardPeriod(localId: string) {
    this.wizardPeriods.update((list) => list.filter((p) => p.localId !== localId));
    this.wizardManualAssignments.set([]);
  }

  // Periods hold a room (In-Person) or a remote URL (Remote) - switching interview type invalidates them.
  clearWizardPeriods() {
    this.wizardPeriods.set([]);
    this.wizardManualAssignments.set([]);
    this.wizardPreview.set(null);
  }

  setWizardManualAssignments(assignments: ManualAssignmentDraft[]) {
    this.wizardManualAssignments.set(assignments);
  }

  setWizardPreview(preview: SchedulePlanPreviewModel | null) {
    this.wizardPreview.set(preview);
  }

  setWizardPreviewLoading(value: boolean) {
    this.wizardPreviewLoading.set(value);
  }

  setWizardSaving(value: boolean) {
    this.wizardSaving.set(value);
  }

  // ---- Detail ----
  setDetailSchedule(schedule: ScheduleModel | null) {
    this.detailSchedule.set(schedule);
    this.detailAppointments.set(schedule?.appointments ?? []);
  }

  setDetailAppointments(appointments: AppointmentModel[]) {
    this.detailAppointments.set(appointments);
  }

  setDetailLoading(value: boolean) {
    this.detailLoading.set(value);
  }
}
