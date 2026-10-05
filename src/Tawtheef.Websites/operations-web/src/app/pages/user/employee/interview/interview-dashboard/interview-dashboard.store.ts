import { computed, inject, Injectable, signal } from '@angular/core';

import { AuthService } from '../../../../../core/auth/auth.service';
import { Permissions } from '../../../../../core/constants/permissions';
import { Lang, LanguageService } from '../../../../../core/services/language.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { LazySelectOption } from '../interview-committees/models/lazy-select-option.model';
import {
  AttendanceView,
  DatePreset,
  DecisionView,
  InterviewDashboardCandidateRow,
  InterviewDashboardFilters,
  InterviewDashboardIssueRow,
  InterviewDashboardOverview,
  InterviewDashboardResultRow,
  InterviewDashboardScheduleRow,
  ReportTab,
} from './models/interview-dashboard.model';
import { OperationalIssueStatus } from '../interview-evaluation/models/enums';

export interface TableState<T> {
  page: PaginatedResult<T> | null;
  pageNumber: number;
  pageSize: number;
  loading: boolean;
  error: boolean;
}

const emptyTable = <T>(): TableState<T> => ({ page: null, pageNumber: 1, pageSize: 10, loading: false, error: false });

@Injectable()
export class InterviewDashboardStore {
  private language = inject(LanguageService);
  private auth = inject(AuthService);

  readonly currentLang = signal<Lang>(this.language.get());
  readonly isRtl = computed(() => this.currentLang() === 'ar');

  // Filters are URL-driven (the facade syncs them) - a language switch reloads the app.
  readonly filters = signal<InterviewDashboardFilters>({});
  readonly datePreset = signal<DatePreset>('all');
  readonly jobOption = signal<LazySelectOption | null>(null);
  readonly committeeOption = signal<LazySelectOption | null>(null);
  readonly scheduleOption = signal<LazySelectOption | null>(null);

  readonly overview = signal<InterviewDashboardOverview | null>(null);
  readonly overviewLoading = signal(false);
  readonly overviewError = signal(false);

  readonly activeTab = signal<ReportTab>('schedules');
  readonly schedules = signal<TableState<InterviewDashboardScheduleRow>>(emptyTable());
  readonly candidates = signal<TableState<InterviewDashboardCandidateRow>>(emptyTable());
  readonly results = signal<TableState<InterviewDashboardResultRow>>(emptyTable());
  readonly issues = signal<TableState<InterviewDashboardIssueRow>>(emptyTable());
  readonly attendanceView = signal<AttendanceView | null>(null);
  readonly decisionView = signal<DecisionView | null>(null);
  readonly issueStatusView = signal<OperationalIssueStatus | null>(null);

  readonly printing = signal(false);

  // Section visibility is decided by the server (overview.sections); before the first response the
  // client-side permission check keeps the layout stable.
  readonly canSeeExecution = computed(
    () =>
      this.overview()?.sections.execution ??
      this.auth.hasPermission([
        Permissions.InterviewSchedule.View,
        Permissions.InterviewSchedule.Manage,
        Permissions.InterviewEvaluation.View,
        Permissions.InterviewEvaluation.Manage,
      ]),
  );
  readonly canSeeResults = computed(
    () =>
      this.overview()?.sections.results ??
      this.auth.hasPermission([Permissions.InterviewResultReport.View, Permissions.InterviewResultReport.Manage]),
  );
  readonly canSeeTemplates = computed(() => this.overview()?.sections.templates ?? false);
  readonly canSeeCommittees = computed(() => this.overview()?.sections.committees ?? false);

  readonly availableTabs = computed<ReportTab[]>(() => [
    ...(this.canSeeExecution() ? (['schedules', 'candidates'] as ReportTab[]) : []),
    ...(this.canSeeResults() ? (['results'] as ReportTab[]) : []),
    ...(this.canSeeExecution() ? (['issues'] as ReportTab[]) : []),
  ]);

  readonly activeFilterCount = computed(() => {
    const f = this.filters();
    return [f.jobId, f.committeeId, f.scheduleId, f.interviewType, f.scheduleStatus, f.resultReportStatus, f.finalDecision]
      .filter((v) => v !== undefined && v !== null).length + (this.datePreset() === 'all' ? 0 : 1);
  });

  localized(ar: string | null | undefined, en: string | null | undefined): string {
    return (this.isRtl() ? ar || en : en || ar) ?? '';
  }

  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }
}
