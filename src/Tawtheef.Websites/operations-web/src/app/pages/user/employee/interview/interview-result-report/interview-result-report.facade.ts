import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { distinctUntilChanged, map } from 'rxjs';

import { NotificationService } from '../../../../../core/services/notification.service';
import { LanguageService } from '../../../../../core/services/language.service';

import { InterviewResultReportStore } from './interview-result-report.store';
import { ApproveResultReportPayload, InterviewResultReportService } from './services/interview-result-report.service';
import { FinalDecision, ResultReportListStatusFilter } from './models/enums';
import { ResultReportListItemModel, ResultReportModel } from './models/result-report.model';

// Same URL-driven-view approach as interview-evaluation: a language switch does a full reload, so the
// open report must live in the URL. Keyed by schedule id - the backend's only detail lookup is
// GET InterviewResultReport/schedule/{scheduleId}.
const SCHEDULE_QUERY_PARAM = 'schedule';

@Injectable()
export class InterviewResultReportFacade {
  private destroyRef = inject(DestroyRef);
  private store = inject(InterviewResultReportStore);
  private api = inject(InterviewResultReportService);
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
        map((params) => params.get(SCHEDULE_QUERY_PARAM)),
        distinctUntilChanged(),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((scheduleId) => this.syncViewWithUrl(scheduleId));
  }

  private syncViewWithUrl(scheduleId: string | null) {
    if (scheduleId) {
      this.store.setView('report');
      this.store.setReport(null);
      this.loadReport(scheduleId);
      return;
    }

    this.store.setView('list');
    this.loadReports();
  }

  private setUrl(scheduleId: string | null) {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { [SCHEDULE_QUERY_PARAM]: scheduleId },
      queryParamsHandling: 'merge',
    });
  }

  localized(ar?: string | null, en?: string | null): string {
    return this.store.isRtl() ? ar || en || '' : en || ar || '';
  }

  // ======== Reports list ========
  loadReports() {
    this.store.listLoading.set(true);
    this.api.list().subscribe({
      next: (items) => {
        this.store.setReportsResult(items);
        this.store.listLoading.set(false);
      },
      error: () => this.store.listLoading.set(false),
    });
  }

  setSearch(value: string) {
    this.store.setSearch(value);
  }

  setStatusFilter(value: ResultReportListStatusFilter | null) {
    this.store.setStatusFilter(value);
  }

  setJobFilter(value: string | null) {
    this.store.setJobFilter(value);
  }

  onPageChange(page: number) {
    this.store.setPage(page);
  }

  onPageSizeChange(size: number) {
    this.store.setPageSize(size);
  }

  // ======== Report (approval) view ========
  openReport(row: ResultReportListItemModel) {
    // Discarded reports are superseded - the schedule URL always resolves to its current report.
    if (row.isDiscarded) return;
    this.setUrl(row.interviewScheduleId);
  }

  backToList() {
    this.setUrl(null);
  }

  loadReport(scheduleId: string) {
    this.store.setReportLoading(true);
    this.api.getBySchedule(scheduleId).subscribe({
      next: (report) => {
        this.store.setReport(report);
        this.seedDecisions(report);
        this.store.setReportLoading(false);
      },
      error: () => {
        this.store.setReportLoading(false);
        this.backToList();
      },
    });
  }

  // Persisted FinalDecision wins (approved report); otherwise the Committee Head's recommendation,
  // then the server-computed SuggestedDecision, pre-fills the dropdown so every candidate always has
  // a selected value.
  private seedDecisions(report: ResultReportModel) {
    const decisions: Record<string, FinalDecision> = {};
    const reasons: Record<string, string> = {};
    for (const c of report.candidates) {
      decisions[c.id] = c.finalDecision ?? c.chairRecommendedDecision ?? c.suggestedDecision;
      if (c.decisionReason) reasons[c.id] = c.decisionReason;
    }
    this.store.setDecisions(decisions);
    this.store.setReasons(reasons);
  }

  setDecision(candidateId: string, decision: FinalDecision) {
    this.store.setDecision(candidateId, decision);
  }

  setReason(candidateId: string, reason: string) {
    this.store.setReason(candidateId, reason);
  }

  // The reason input only shows while the reviewer overrides the suggestion (see report-detail.html),
  // so a reason typed and then made moot by switching back to the suggestion is not sent.
  approve() {
    const report = this.store.report();
    if (!report) return;

    if (!this.store.allDecided()) {
      this.notify.warn(this.translate.instant('INTERVIEW_RESULT_REPORT.DETAIL.DECISIONS_REQUIRED'));
      return;
    }

    const decisions = this.store.decisions();
    const reasons = this.store.reasons();
    const payload: ApproveResultReportPayload = {
      reportId: report.id,
      decisions: report.candidates.map((c) => {
        const decision = decisions[c.id];
        const reason = decision !== c.suggestedDecision ? reasons[c.id]?.trim() : '';
        return { candidateId: c.id, decision, reason: reason || null };
      }),
    };

    this.store.setApproving(true);
    this.api.approve(payload).subscribe({
      next: () => {
        this.store.setApproving(false);
        this.notify.success(this.translate.instant('INTERVIEW_RESULT_REPORT.DETAIL.APPROVED_SUCCESS'));
        this.loadReport(report.interviewScheduleId);
      },
      error: () => this.store.setApproving(false),
    });
  }
}
