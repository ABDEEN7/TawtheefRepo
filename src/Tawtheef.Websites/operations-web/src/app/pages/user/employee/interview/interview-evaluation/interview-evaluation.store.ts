import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';

import { AppointmentModel } from '../interview-schedule/models/appointment.model';
import { InterviewType, ScheduleStatus } from '../interview-schedule/models/enums';
import { ScheduleModel } from '../interview-schedule/models/schedule.model';

import { FinalDecision, ResultReportStatus } from '../interview-result-report/models/enums';

import { SessionListRowModel } from './models/session-row.model';
import {
  CommitteeReviewListItemModel,
  CommitteeReviewModel,
  SchoolStageOption,
} from './models/committee-review.model';
import { OperationalIssueModel } from './models/operational-issue.model';
import {
  AppointmentEvaluationContextModel,
  AppointmentEvaluationSummaryModel,
  MemberEvaluationFormModel,
} from './models/evaluation.model';

export type EvaluationView = 'list' | 'session' | 'candidate' | 'review';

// Report statuses in which the chair can still open and edit the Committee Head Review - once the
// report is approved (or closed) the Committee Review action is hidden.
export const COMMITTEE_REVIEW_EDITABLE_STATUSES: ResultReportStatus[] = [
  ResultReportStatus.CommitteeReview,
  ResultReportStatus.UnderReview,
];

// Schedules whose interviewing can be running or has run - the "Start Interview" list only ever
// shows these (see interview-evaluation.facade.ts loadSessions). Closed stays reachable only via
// the Status filter, not shown by default.
export const ELIGIBLE_SESSION_STATUSES: ScheduleStatus[] = [
  ScheduleStatus.Approved,
  ScheduleStatus.ReadyForExecution,
  ScheduleStatus.InProgress,
  ScheduleStatus.Closed,
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
      const label = this.isRtl()
        ? s.jobTitleNameAr || s.jobTitleNameEn
        : s.jobTitleNameEn || s.jobTitleNameAr;
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
      return [s.jobTitleNameAr, s.jobTitleNameEn].some((value) =>
        (value ?? '').toLowerCase().includes(term),
      );
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

  // ======== Committee Head Review ========
  // Reports the caller may review, keyed by schedule id. A schedule missing here has no report yet
  // (not every candidate is done) or the caller isn't its chair.
  committeeReviews = signal<Record<string, CommitteeReviewListItemModel>>({});
  review = signal<CommitteeReviewModel | null>(null);
  reviewLoading = signal(false);
  reviewSaving = signal(false);
  schoolStages = signal<SchoolStageOption[]>([]);
  // Chair's working selection, keyed by candidate id - seeded from the saved recommendation, or the
  // system suggestion when the chair hasn't overridden it (see facade seedReview).
  reviewDecisions = signal<Record<string, FinalDecision>>({});
  reviewReasons = signal<Record<string, string>>({});
  reviewStages = signal<Record<string, string | null>>({});

  reviewEditable = computed(() => this.review()?.canEdit ?? false);
  reviewInCommitteeStage = computed(
    () => this.review()?.status === ResultReportStatus.CommitteeReview,
  );

  // The Committee Review action for a schedule: 'hidden' once its report is approved/closed,
  // 'disabled' while there's no report the caller can review, 'enabled' otherwise.
  committeeReviewState(scheduleId: string | null | undefined): 'enabled' | 'disabled' | 'hidden' {
    const entry = scheduleId ? this.committeeReviews()[scheduleId] : undefined;
    if (!entry) return 'disabled';
    return COMMITTEE_REVIEW_EDITABLE_STATUSES.includes(entry.status) ? 'enabled' : 'hidden';
  }

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

  // ---- Committee Head Review ----
  setCommitteeReviews(items: CommitteeReviewListItemModel[]) {
    this.committeeReviews.set(Object.fromEntries(items.map((i) => [i.interviewScheduleId, i])));
  }

  setReview(review: CommitteeReviewModel | null) {
    this.review.set(review);
  }

  setReviewLoading(value: boolean) {
    this.reviewLoading.set(value);
  }

  setReviewSaving(value: boolean) {
    this.reviewSaving.set(value);
  }

  setSchoolStages(stages: SchoolStageOption[]) {
    this.schoolStages.set(stages);
  }

  seedReview(
    decisions: Record<string, FinalDecision>,
    reasons: Record<string, string>,
    stages: Record<string, string | null>,
  ) {
    this.reviewDecisions.set(decisions);
    this.reviewReasons.set(reasons);
    this.reviewStages.set(stages);
  }

  setReviewDecision(candidateId: string, decision: FinalDecision) {
    this.reviewDecisions.update((map) => ({ ...map, [candidateId]: decision }));
  }

  setReviewReason(candidateId: string, reason: string) {
    this.reviewReasons.update((map) => ({ ...map, [candidateId]: reason }));
  }

  setReviewStage(candidateId: string, stageId: string | null) {
    this.reviewStages.update((map) => ({ ...map, [candidateId]: stageId }));
  }
}
