import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';

import { AppointmentModel } from '../interview-schedule/models/appointment.model';
import { InterviewType, ScheduleStatus } from '../interview-schedule/models/enums';
import { ScheduleModel } from '../interview-schedule/models/schedule.model';

import { SessionListRowModel } from './models/session-row.model';
import { OperationalIssueModel } from './models/operational-issue.model';
import {
  AppointmentEvaluationContextModel,
  AppointmentEvaluationSummaryModel,
  MemberEvaluationFormModel,
} from './models/evaluation.model';

export type EvaluationView = 'list' | 'session' | 'candidate';

// Schedules whose interviewing can be running or has run - the "Start Interview" list only ever
// shows these (see interview-evaluation.facade.ts loadSessions). Closed stays reachable only via
// the Status filter, not shown by default.
export const ELIGIBLE_SESSION_STATUSES: ScheduleStatus[] = [
  ScheduleStatus.Approved,
  ScheduleStatus.ReadyForExecution,
  ScheduleStatus.InProgress,
];

@Injectable()
export class InterviewEvaluationStore {
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  view = signal<EvaluationView>('list');

  // ======== Sessions list ========
  private sessionsResult = signal<SessionListRowModel[]>([]);
  listLoading = signal(false);
  search = signal('');
  statusFilter = signal<ScheduleStatus | null>(null);
  jobFilter = signal<string | null>(null);
  typeFilter = signal<InterviewType | null>(null);

  jobOptions = computed(() => {
    const options = new Map<string, { value: string; label: string }>();
    for (const s of this.sessionsResult()) {
      if (!s.jobTitleId || options.has(s.jobTitleId)) continue;
      const label = this.isRtl() ? s.jobTitleNameAr || s.jobTitleNameEn : s.jobTitleNameEn || s.jobTitleNameAr;
      if (label) options.set(s.jobTitleId, { value: s.jobTitleId, label });
    }
    return [...options.values()].sort((a, b) => a.label.localeCompare(b.label));
  });

  page = signal(1);
  pageSize = signal(20);

  // Eligible-for-evaluation statuses only, further narrowed by the Status filter (which itself can
  // only be one of ELIGIBLE_SESSION_STATUSES or Closed - see sessions-list.ts statusOptions).
  sessions = computed(() => {
    const term = this.search().trim().toLowerCase();
    const status = this.statusFilter();
    const jobTitleId = this.jobFilter();
    const type = this.typeFilter();
    return this.sessionsResult().filter((s) => {
      if (status !== null) {
        if (s.status !== status) return false;
      } else if (!ELIGIBLE_SESSION_STATUSES.includes(s.status)) {
        return false;
      }
      if (jobTitleId !== null && s.jobTitleId !== jobTitleId) return false;
      if (type !== null && s.defaultInterviewType !== type) return false;
      if (!term) return true;
      return [s.jobTitleNameAr, s.jobTitleNameEn].some((value) => (value ?? '').toLowerCase().includes(term));
    });
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.sessions().length / this.pageSize())));
  currentPage = computed(() => Math.min(this.page(), this.totalPages()));

  pagedSessions = computed(() => {
    const size = this.pageSize();
    const start = (this.currentPage() - 1) * size;
    return this.sessions().slice(start, start + size);
  });

  // ======== Session (appointment) view ========
  detailSchedule = signal<ScheduleModel | null>(null);
  detailAppointments = signal<AppointmentModel[]>([]);
  detailLoading = signal(false);
  // Per-appointment evaluation summary, keyed by appointment id - null once resolved means "no
  // access" (403 for a plain evaluator), absent means "not fetched yet".
  appointmentSummaries = signal<Record<string, AppointmentEvaluationSummaryModel | null>>({});
  // Per-appointment operational issues, keyed by appointment id - powers the issues-button badge on
  // the session table (see session-detail.ts). Absent means "not fetched yet".
  appointmentIssues = signal<Record<string, OperationalIssueModel[]>>({});

  // ======== Candidate evaluation page ========
  selectedAppointmentId = signal<string | null>(null);
  candidateContext = signal<AppointmentEvaluationContextModel | null>(null);
  candidateForm = signal<MemberEvaluationFormModel | null>(null);
  candidateSummary = signal<AppointmentEvaluationSummaryModel | null>(null);
  candidateLoading = signal(false);
  candidateSaving = signal(false);
  candidateIssues = signal<OperationalIssueModel[]>([]);
  candidateIssuesLoading = signal(false);

  selectedAppointment = computed(() => {
    const id = this.selectedAppointmentId();
    return id ? (this.detailAppointments().find((a) => a.id === id) ?? null) : null;
  });

  // ---- setters ----
  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setView(view: EvaluationView) {
    this.view.set(view);
  }

  setSessionsResult(items: SessionListRowModel[]) {
    this.sessionsResult.set(items);
  }

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

  // ---- Session view ----
  setDetailSchedule(schedule: ScheduleModel | null) {
    this.detailSchedule.set(schedule);
    this.detailAppointments.set(schedule?.appointments ?? []);
  }

  setDetailLoading(value: boolean) {
    this.detailLoading.set(value);
  }

  // Refreshes just the appointment rows (status/attendance can change from the candidate page)
  // without refetching the whole schedule - see refreshDetailAppointments in the facade.
  setDetailAppointments(appointments: AppointmentModel[]) {
    this.detailAppointments.set(appointments);
  }

  setAppointmentSummary(appointmentId: string, summary: AppointmentEvaluationSummaryModel | null) {
    this.appointmentSummaries.update((map) => ({ ...map, [appointmentId]: summary }));
  }

  clearAppointmentSummaries() {
    this.appointmentSummaries.set({});
  }

  setAppointmentIssues(appointmentId: string, issues: OperationalIssueModel[]) {
    this.appointmentIssues.update((map) => ({ ...map, [appointmentId]: issues }));
  }

  clearAppointmentIssues() {
    this.appointmentIssues.set({});
  }

  // ---- Candidate view ----
  setSelectedAppointment(appointmentId: string | null) {
    this.selectedAppointmentId.set(appointmentId);
  }

  setCandidateContext(context: AppointmentEvaluationContextModel | null) {
    this.candidateContext.set(context);
  }

  setCandidateForm(form: MemberEvaluationFormModel | null) {
    this.candidateForm.set(form);
  }

  setCandidateSummary(summary: AppointmentEvaluationSummaryModel | null) {
    this.candidateSummary.set(summary);
  }

  setCandidateLoading(value: boolean) {
    this.candidateLoading.set(value);
  }

  setCandidateSaving(value: boolean) {
    this.candidateSaving.set(value);
  }

  setCandidateIssues(issues: OperationalIssueModel[]) {
    this.candidateIssues.set(issues);
  }

  setCandidateIssuesLoading(value: boolean) {
    this.candidateIssuesLoading.set(value);
  }

  resetCandidate() {
    this.candidateContext.set(null);
    this.candidateForm.set(null);
    this.candidateSummary.set(null);
    this.candidateIssues.set([]);
  }
}
