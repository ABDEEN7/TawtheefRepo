import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError, distinctUntilChanged, forkJoin, map, of } from 'rxjs';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';
import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';

import { InterviewScheduleService } from '../interview-schedule/services/interview-schedule.service';
import { AppointmentModel } from '../interview-schedule/models/appointment.model';
import { AppointmentStatus, InterviewType, ScheduleStatus } from '../interview-schedule/models/enums';
import { ScheduleListItemModel } from '../interview-schedule/models/schedule.model';

import { InterviewEvaluationStore } from './interview-evaluation.store';
import {
  CreateOperationalIssuePayload,
  InterviewEvaluationService,
  SaveMemberEvaluationDraftPayload,
} from './services/interview-evaluation.service';
import { AttendanceStatus } from './models/enums';
import { SessionListRowModel } from './models/session-row.model';
import { CommitteeReviewModel, SaveCommitteeReviewPayload } from './models/committee-review.model';
import { CommitteeReviewService } from './services/committee-review.service';
import { FinalDecision } from '../interview-result-report/models/enums';

// Which appointment statuses count as "evaluation done" for the sessions list Progress column.
const COMPLETED_APPOINTMENT_STATUSES = [AppointmentStatus.Completed, AppointmentStatus.Closed];
// Appointments that no longer represent a live, evaluable candidate slot on the schedule.
const INACTIVE_APPOINTMENT_STATUSES = [AppointmentStatus.Cancelled, AppointmentStatus.Rescheduled];

// Same URL-driven-view fix already applied to interview-schedule (see language-switch-reloads-app
// memory): a language switch does a full reload, so the open session/candidate must live in the
// URL, not only in the store, or the user is dropped back on the list.
const SESSION_QUERY_PARAM = 'schedule';
const APPOINTMENT_QUERY_PARAM = 'appointment';
// ?schedule=<id>&review=1 opens the Committee Head Review of that schedule's result report.
const REVIEW_QUERY_PARAM = 'review';

@Injectable()
export class InterviewEvaluationFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(InterviewEvaluationStore);
  private scheduleApi = inject(InterviewScheduleService);
  private evaluationApi = inject(InterviewEvaluationService);
  private committeeReviewApi = inject(CommitteeReviewService);
  private auth = inject(AuthService);
  private notify = inject(NotificationService);
  private translate = inject(TranslateService);
  private language = inject(LanguageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  init() {
    this.store.setCurrentLang(this.language.get());
    this.language.current$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((lang) => this.store.setCurrentLang(lang));

    this.route.queryParamMap
      .pipe(
        map((params) => ({
          schedule: params.get(SESSION_QUERY_PARAM),
          appointment: params.get(APPOINTMENT_QUERY_PARAM),
          review: params.get(REVIEW_QUERY_PARAM) === '1',
        })),
        distinctUntilChanged((a, b) => a.schedule === b.schedule && a.appointment === b.appointment && a.review === b.review),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(({ schedule, appointment, review }) => this.syncViewWithUrl(schedule, appointment, review));

    // Also loads the Committee Review availability (see loadSessions).
    this.loadSessions();
  }

  private syncViewWithUrl(scheduleId: string | null, appointmentId: string | null, review: boolean) {
    if (scheduleId && review) {
      if (this.store.view() === 'review' && this.store.review()?.interviewScheduleId === scheduleId) return;
      this.store.setView('review');
      this.loadReview(scheduleId);
      return;
    }

    if (scheduleId && appointmentId) {
      if (this.store.view() === 'candidate' && this.store.selectedAppointmentId() === appointmentId) return;
      this.store.setView('candidate');
      this.store.setSelectedAppointment(appointmentId);
      this.ensureSessionLoaded(scheduleId, () => this.loadCandidate(appointmentId));
      return;
    }

    if (scheduleId) {
      if (this.store.view() === 'session' && this.store.detailSchedule()?.id === scheduleId) return;
      this.store.setView('session');
      this.store.setDetailSchedule(null);
      this.loadSessionDetail(scheduleId);
      return;
    }

    if (this.store.view() !== 'list') {
      this.store.setView('list');
      this.loadSessions();
    }
  }

  // The candidate page can be opened directly (deep link / reload) without the session view ever
  // having loaded the schedule first.
  private ensureSessionLoaded(scheduleId: string, then: () => void) {
    if (this.store.detailSchedule()?.id === scheduleId) {
      then();
      return;
    }
    this.loadSessionDetail(scheduleId, then);
  }

  private setUrl(scheduleId: string | null, appointmentId: string | null, review = false) {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        [SESSION_QUERY_PARAM]: scheduleId,
        [APPOINTMENT_QUERY_PARAM]: appointmentId,
        [REVIEW_QUERY_PARAM]: review ? '1' : null,
      },
      queryParamsHandling: 'merge',
    });
  }

  localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  // ======== Sessions list ========
  loadSessions() {
    this.store.listLoading.set(true);
    this.loadCommitteeReviews();
    this.evaluationApi.listMySessions().subscribe({
      next: (items) => this.enrichAndSetSessions(items),
      error: () => this.store.listLoading.set(false),
    });
  }

  // Progress (completed/total appointments) and Location (distinct room names) aren't on
  // ScheduleListItemDto - fetched per schedule and derived client-side (pure display arithmetic,
  // not a duplicated domain rule). See models/session-row.model.ts.
  private enrichAndSetSessions(items: ScheduleListItemModel[]) {
    if (items.length === 0) {
      this.store.setSessionsResult([]);
      this.store.listLoading.set(false);
      return;
    }

    forkJoin(
      items.map((item) =>
        this.evaluationApi.listMySessionAppointments(item.id).pipe(
          map((appointments) => this.toSessionRow(item, appointments)),
          catchError(() => of(this.toSessionRow(item, []))),
        ),
      ),
    ).subscribe({
      next: (rows) => {
        this.store.setSessionsResult(rows);
        this.store.listLoading.set(false);
      },
      error: () => this.store.listLoading.set(false),
    });
  }

  private toSessionRow(item: ScheduleListItemModel, appointments: AppointmentModel[]): SessionListRowModel {
    const live = appointments.filter((a) => !!a.invitationId && !INACTIVE_APPOINTMENT_STATUSES.includes(a.status));
    const completed = live.filter((a) => COMPLETED_APPOINTMENT_STATUSES.includes(a.status));
    const roomNames = [...new Set(live.filter((a) => a.roomId).map((a) => this.localized(a.roomNameAr, a.roomNameEn)))];
    return {
      ...item,
      completedAppointmentsCount: completed.length,
      totalAssignedAppointmentsCount: live.length,
      roomNames,
    };
  }

  setSearch(value: string) {
    this.store.setSearch(value);
  }

  setStatusFilter(value: ScheduleStatus | null) {
    this.store.setStatusFilter(value);
  }

  setJobFilter(value: string | null) {
    this.store.setJobFilter(value);
  }

  setTypeFilter(value: InterviewType | null) {
    this.store.setTypeFilter(value);
  }

  onPageChange(page: number) {
    this.store.setPage(page);
  }

  onPageSizeChange(size: number) {
    this.store.setPageSize(size);
  }

  // ======== Session (appointment) view ========
  openSession(row: SessionListRowModel) {
    this.setUrl(row.id, null);
  }

  backToList() {
    this.setUrl(null, null);
  }

  loadSessionDetail(scheduleId: string, then?: () => void) {
    this.store.setDetailLoading(true);
    this.store.clearAppointmentSummaries();
    this.store.clearAppointmentIssues();
    this.evaluationApi.getMySession(scheduleId).subscribe({
      next: (schedule) => {
        this.store.setDetailSchedule(schedule);
        this.store.setDetailLoading(false);
        this.loadAppointmentSummaries(schedule.appointments);
        this.loadAppointmentIssues(schedule.appointments);
        then?.();
      },
      error: () => {
        this.store.setDetailLoading(false);
        this.backToList();
      },
    });
  }

  // Best-effort per-appointment progress for the session table - a plain evaluator 403s on some/all
  // of these (only Chair/CanViewCommitteeSummary/HR see a committee's whole picture), so a failure
  // just leaves that row's summary as "unavailable", never a toast (see SKIP_ERROR in the service).
  private loadAppointmentSummaries(appointments: AppointmentModel[]) {
    const withCandidates = appointments.filter((a) => !!a.invitationId);
    if (withCandidates.length === 0) return;

    for (const appointment of withCandidates) {
      this.evaluationApi
        .getSummary(appointment.id)
        .pipe(catchError(() => of(null)))
        .subscribe((summary) => this.store.setAppointmentSummary(appointment.id, summary));
    }
  }

  // Powers the operational-issues badge on the session table so a user can see an issue exists
  // without opening the dialog first (see session-detail.ts issue count helpers).
  private loadAppointmentIssues(appointments: AppointmentModel[]) {
    const withCandidates = appointments.filter((a) => !!a.invitationId);
    if (withCandidates.length === 0) return;

    for (const appointment of withCandidates) {
      this.refreshAppointmentIssues(appointment.id);
    }
  }

  refreshAppointmentIssues(appointmentId: string) {
    this.evaluationApi
      .listOperationalIssues(appointmentId)
      .pipe(catchError(() => of([])))
      .subscribe((issues) => this.store.setAppointmentIssues(appointmentId, issues));
  }

  // ======== Candidate evaluation page ========
  openCandidate(appointmentId: string) {
    const scheduleId = this.store.detailSchedule()?.id;
    if (!scheduleId) return;
    this.setUrl(scheduleId, appointmentId);
  }

  backToSession() {
    const scheduleId = this.store.detailSchedule()?.id;
    this.setUrl(scheduleId ?? null, null);
  }

  loadCandidate(appointmentId: string) {
    this.store.setCandidateLoading(true);
    this.store.resetCandidate();

    this.evaluationApi
      .getContext(appointmentId)
      .pipe(catchError(() => of(null)))
      .subscribe((context) => {
        this.store.setCandidateContext(context);
        if (context?.hasChairOrBypassAccess) this.loadCandidateSummary(appointmentId);
      });

    this.evaluationApi.getForm(appointmentId).subscribe({
      next: (form) => {
        this.store.setCandidateForm(form);
        this.store.setCandidateLoading(false);
      },
      error: () => this.store.setCandidateLoading(false),
    });

    this.loadOperationalIssues(appointmentId);
  }

  private loadCandidateSummary(appointmentId: string) {
    this.evaluationApi
      .getSummary(appointmentId)
      .pipe(catchError(() => of(null)))
      .subscribe((summary) => this.store.setCandidateSummary(summary));
  }

  // Any mutation reachable from the candidate page (attendance -> StartInterview, submit -> possible
  // CompleteEvaluation quorum trigger) can change the appointment's own Status/AttendanceStatus - both
  // live on AppointmentModel, sourced from the session view's appointment list, not from the
  // candidate-only calls above. Without this, the header (and the session view once the user goes
  // back) would keep showing the pre-mutation status.
  reloadCandidate() {
    const appointmentId = this.store.selectedAppointmentId();
    if (appointmentId) this.loadCandidate(appointmentId);
    this.refreshDetailAppointments();
    // The mutation may have finished the schedule and generated its result report.
    this.loadCommitteeReviews();
  }

  private refreshDetailAppointments() {
    const scheduleId = this.store.detailSchedule()?.id;
    if (!scheduleId) return;
    this.evaluationApi.listMySessionAppointments(scheduleId).subscribe((appointments) => this.store.setDetailAppointments(appointments));
  }

  // ======== Attendance ========
  // Registering Present chains StartAppointmentInterview right after (Scheduled -> InInterview,
  // domain-required before axes unlock). Absent (NoShow) closes the appointment server-side
  // (InterviewAppointment.ApplyAttendanceOutcome) - axes stay locked for good.
  registerAttendance(appointmentId: string, status: AttendanceStatus) {
    this.scheduleApi.recordAppointmentAttendance(appointmentId, status).subscribe({
      next: () => {
        if (status !== AttendanceStatus.Present) {
          this.toast('INTERVIEW_EVALUATION.CANDIDATE.ATTENDANCE_SAVED');
          this.reloadCandidate();
          return;
        }
        this.scheduleApi.startAppointmentInterview(appointmentId).subscribe({
          next: () => {
            this.toast('INTERVIEW_EVALUATION.CANDIDATE.ATTENDANCE_SAVED');
            this.reloadCandidate();
          },
          error: () => this.reloadCandidate(),
        });
      },
    });
  }

  // ======== Scoring ========
  saveDraft(payload: SaveMemberEvaluationDraftPayload) {
    this.store.setCandidateSaving(true);
    this.evaluationApi.saveDraft(payload).subscribe({
      next: () => {
        this.toast('INTERVIEW_EVALUATION.CANDIDATE.DRAFT_SAVED');
        this.store.setCandidateSaving(false);
        this.reloadCandidate();
      },
      error: () => this.store.setCandidateSaving(false),
    });
  }

  // SubmitMemberEvaluationCommand finalizes whatever is already persisted as a draft - it takes no
  // scores of its own. Saving the current in-memory scores first (then submitting only once that
  // succeeds) is required so the user's last edits before clicking Submit are actually the ones
  // evaluated, not a stale earlier draft. This is call sequencing, not a re-implemented validation
  // rule - the server still does all the required-criterion/scope checking on submit.
  submitEvaluation(payload: SaveMemberEvaluationDraftPayload) {
    this.store.setCandidateSaving(true);
    this.evaluationApi.saveDraft(payload).subscribe({
      next: () => {
        this.evaluationApi.submit(payload.appointmentId).subscribe({
          next: () => {
            this.toast('INTERVIEW_EVALUATION.CANDIDATE.SUBMITTED');
            this.store.setCandidateSaving(false);
            this.reloadCandidate();
          },
          error: () => this.store.setCandidateSaving(false),
        });
      },
      error: () => this.store.setCandidateSaving(false),
    });
  }

  // ======== Operational issues ========
  loadOperationalIssues(appointmentId: string) {
    this.store.setCandidateIssuesLoading(true);
    this.evaluationApi.listOperationalIssues(appointmentId).subscribe({
      next: (issues) => {
        this.store.setCandidateIssues(issues);
        this.store.setCandidateIssuesLoading(false);
      },
      error: () => this.store.setCandidateIssuesLoading(false),
    });
  }

  createOperationalIssue(payload: CreateOperationalIssuePayload) {
    this.evaluationApi.createOperationalIssue(payload).subscribe({
      next: () => {
        this.toast('INTERVIEW_EVALUATION.ISSUES.CREATED');
        this.loadOperationalIssues(payload.appointmentId);
      },
    });
  }

  resolveOperationalIssue(appointmentId: string, issueId: string, resolutionNotes: string | null) {
    this.evaluationApi.resolveOperationalIssue(issueId, resolutionNotes).subscribe({
      next: () => {
        this.toast('INTERVIEW_EVALUATION.ISSUES.RESOLVED');
        this.loadOperationalIssues(appointmentId);
      },
    });
  }

  waiveOperationalIssue(appointmentId: string, issueId: string, resolutionNotes: string | null) {
    this.evaluationApi.waiveOperationalIssue(issueId, resolutionNotes).subscribe({
      next: () => {
        this.toast('INTERVIEW_EVALUATION.ISSUES.WAIVED');
        this.loadOperationalIssues(appointmentId);
      },
    });
  }

  toggleOperationalIssueBlocking(appointmentId: string, issueId: string, isBlocking: boolean) {
    this.evaluationApi.updateOperationalIssueBlocking(issueId, isBlocking).subscribe({
      next: () => this.loadOperationalIssues(appointmentId),
    });
  }

  // ======== Committee Head Review ========
  // Which schedules already have a report this user can review - drives the Committee Review action
  // on the sessions list, the session view and the evaluation form.
  loadCommitteeReviews() {
    if (!this.auth.hasPermission(Permissions.InterviewCommitteeReview.View)) return;
    this.committeeReviewApi
      .list()
      .pipe(catchError(() => of([])))
      .subscribe((items) => this.store.setCommitteeReviews(items));
  }

  openCommitteeReview(scheduleId: string) {
    if (this.store.committeeReviewState(scheduleId) !== 'enabled') return;
    this.setUrl(scheduleId, null, true);
  }

  backFromReview() {
    this.setUrl(this.store.review()?.interviewScheduleId ?? null, null);
  }

  loadReview(scheduleId: string) {
    this.store.setReviewLoading(true);
    this.store.setReview(null);
    if (this.store.schoolStages().length === 0) {
      this.committeeReviewApi
        .schoolStages()
        .pipe(catchError(() => of([])))
        .subscribe((stages) => this.store.setSchoolStages(stages));
    }
    this.committeeReviewApi.getBySchedule(scheduleId).subscribe({
      next: (review) => {
        this.store.setReview(review);
        this.seedReview(review);
        this.store.setReviewLoading(false);
      },
      error: () => {
        this.store.setReviewLoading(false);
        this.backToList();
      },
    });
  }

  // The chair's saved recommendation wins; otherwise the system suggestion pre-fills the dropdown.
  private seedReview(review: CommitteeReviewModel) {
    const decisions: Record<string, FinalDecision> = {};
    const reasons: Record<string, string> = {};
    const stages: Record<string, string | null> = {};
    for (const c of review.candidates) {
      decisions[c.id] = c.chairRecommendedDecision ?? c.suggestedDecision;
      if (c.chairRecommendationReason) reasons[c.id] = c.chairRecommendationReason;
      stages[c.id] = c.recommendedSchoolStageId;
    }
    this.store.seedReview(decisions, reasons, stages);
  }

  setReviewDecision(candidateId: string, decision: FinalDecision) {
    this.store.setReviewDecision(candidateId, decision);
  }

  setReviewReason(candidateId: string, reason: string) {
    this.store.setReviewReason(candidateId, reason);
  }

  setReviewStage(candidateId: string, stageId: string | null) {
    this.store.setReviewStage(candidateId, stageId);
  }

  // Picking the suggestion again clears the override (recommendedDecision null). Without the override
  // permission the saved recommendation is sent back unchanged - the server rejects any change anyway.
  saveReview(sendForApproval: boolean) {
    const review = this.store.review();
    if (!review) return;

    const decisions = this.store.reviewDecisions();
    const reasons = this.store.reviewReasons();
    const stages = this.store.reviewStages();
    const payload: SaveCommitteeReviewPayload = {
      reportId: review.reportId,
      sendForApproval,
      candidates: review.candidates.map((c) => {
        if (!review.canOverrideSuggestion) {
          return {
            candidateId: c.id,
            recommendedDecision: c.chairRecommendedDecision,
            reason: c.chairRecommendationReason,
            schoolStageId: stages[c.id] ?? null,
          };
        }
        const decision = decisions[c.id];
        const overridden = decision !== c.suggestedDecision;
        return {
          candidateId: c.id,
          recommendedDecision: overridden ? decision : null,
          reason: overridden ? reasons[c.id]?.trim() || null : null,
          schoolStageId: stages[c.id] ?? null,
        };
      }),
    };

    this.store.setReviewSaving(true);
    this.committeeReviewApi.save(payload).subscribe({
      next: () => {
        this.store.setReviewSaving(false);
        this.toast(sendForApproval ? 'INTERVIEW_EVALUATION.REVIEW.SENT_SUCCESS' : 'INTERVIEW_EVALUATION.REVIEW.SAVED_SUCCESS');
        this.loadCommitteeReviews();
        this.loadReview(review.interviewScheduleId);
      },
      error: () => this.store.setReviewSaving(false),
    });
  }

  // ======== Errors/toasts ========
  translateServerError(err: HttpErrorResponse): string {
    const body = err.error as { error?: { message?: string }[]; detail?: string } | null;
    const code = body?.error?.[0]?.message || body?.detail || 'UN_EXPECTED_ERROR';
    const key = `server-error.${code}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : this.translate.instant('server-error.UN_EXPECTED_ERROR');
  }

  private toast(key: string) {
    this.notify.success(this.translate.instant(key));
  }
}
