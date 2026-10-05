import { Injectable, computed, inject, signal } from '@angular/core';

import { LanguageService, Lang } from '../../../../../core/services/language.service';

import { DISCARDED_REPORT_FILTER, FinalDecision, ResultReportListStatusFilter, ResultReportStatus } from './models/enums';
import { ResultReportListItemModel, ResultReportModel } from './models/result-report.model';

export type ResultReportView = 'list' | 'report';

@Injectable()
export class InterviewResultReportStore {
  private language = inject(LanguageService);

  currentLang = signal<Lang>(this.language.get());
  isRtl = computed(() => this.currentLang() === 'ar');

  view = signal<ResultReportView>('list');

  // ======== Reports list ========
  private reportsResult = signal<ResultReportListItemModel[]>([]);
  listLoading = signal(false);
  search = signal('');
  statusFilter = signal<ResultReportListStatusFilter | null>(null);
  jobFilter = signal<string | null>(null);

  // ResultReportListItemDto carries no JobId - the job filter keys on the Arabic job name (always
  // present) and shows the localized one.
  jobOptions = computed(() => {
    const options = new Map<string, { value: string; label: string }>();
    for (const r of this.reportsResult()) {
      if (!r.jobNameAr || options.has(r.jobNameAr)) continue;
      const label = this.isRtl() ? r.jobNameAr : r.jobNameEn || r.jobNameAr;
      options.set(r.jobNameAr, { value: r.jobNameAr, label });
    }
    return [...options.values()].sort((a, b) => a.label.localeCompare(b.label));
  });

  page = signal(1);
  pageSize = signal(20);

  reports = computed(() => {
    const term = this.search().trim().toLowerCase();
    const status = this.statusFilter();
    const job = this.jobFilter();
    return this.reportsResult().filter((r) => {
      // A discarded report keeps its last status but is only shown under the "Discarded" filter.
      if (status === DISCARDED_REPORT_FILTER && !r.isDiscarded) return false;
      if (status !== null && status !== DISCARDED_REPORT_FILTER && (r.isDiscarded || r.status !== status)) return false;
      if (job !== null && r.jobNameAr !== job) return false;
      if (!term) return true;
      return [r.code, r.jobNameAr, r.jobNameEn, r.scheduleTitleAr, r.scheduleTitleEn].some((value) =>
        (value ?? '').toLowerCase().includes(term),
      );
    });
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.reports().length / this.pageSize())));
  currentPage = computed(() => Math.min(this.page(), this.totalPages()));

  pagedReports = computed(() => {
    const size = this.pageSize();
    const start = (this.currentPage() - 1) * size;
    return this.reports().slice(start, start + size);
  });

  // ======== Report (approval) view ========
  report = signal<ResultReportModel | null>(null);
  reportLoading = signal(false);
  approving = signal(false);
  // Reviewer's working selection, keyed by candidate id - seeded from the persisted FinalDecision, or
  // the server's SuggestedDecision while the report is still under review (see facade seedDecisions).
  decisions = signal<Record<string, FinalDecision>>({});
  reasons = signal<Record<string, string>>({});

  isUnderReview = computed(() => this.report()?.status === ResultReportStatus.UnderReview);

  allDecided = computed(() => {
    const report = this.report();
    if (!report || report.candidates.length === 0) return false;
    const decisions = this.decisions();
    return report.candidates.every((c) => decisions[c.id] != null);
  });

  // ---- setters ----
  setCurrentLang(lang: Lang) {
    this.currentLang.set(lang);
  }

  setView(view: ResultReportView) {
    this.view.set(view);
  }

  setReportsResult(items: ResultReportListItemModel[]) {
    this.reportsResult.set(items);
  }

  setSearch(value: string) {
    this.search.set(value ?? '');
    this.page.set(1);
  }

  setStatusFilter(value: ResultReportListStatusFilter | null) {
    this.statusFilter.set(value);
    this.page.set(1);
  }

  setJobFilter(value: string | null) {
    this.jobFilter.set(value);
    this.page.set(1);
  }

  setPage(page: number) {
    this.page.set(page);
  }

  setPageSize(size: number) {
    this.pageSize.set(size);
    this.page.set(1);
  }

  // ---- Report view ----
  setReport(report: ResultReportModel | null) {
    this.report.set(report);
  }

  setReportLoading(value: boolean) {
    this.reportLoading.set(value);
  }

  setApproving(value: boolean) {
    this.approving.set(value);
  }

  setDecisions(decisions: Record<string, FinalDecision>) {
    this.decisions.set(decisions);
  }

  setDecision(candidateId: string, decision: FinalDecision) {
    this.decisions.update((map) => ({ ...map, [candidateId]: decision }));
  }

  setReasons(reasons: Record<string, string>) {
    this.reasons.set(reasons);
  }

  setReason(candidateId: string, reason: string) {
    this.reasons.update((map) => ({ ...map, [candidateId]: reason }));
  }
}
