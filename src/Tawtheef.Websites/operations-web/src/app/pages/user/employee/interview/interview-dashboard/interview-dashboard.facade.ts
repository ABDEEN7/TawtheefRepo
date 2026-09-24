import { DestroyRef, inject, Injectable } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { catchError, map, Observable, of, Subject, switchMap } from 'rxjs';

import { LanguageService } from '../../../../../core/services/language.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { LazySelectOption } from '../interview-committees/models/lazy-select-option.model';
import { OperationalIssueStatus } from '../interview-evaluation/models/enums';
import { InterviewDashboardStore, TableState } from './interview-dashboard.store';
import {
  AttendanceView,
  DatePreset,
  DecisionView,
  InterviewDashboardFilters,
  InterviewDashboardLookup,
  InterviewDashboardLookupKind,
  ReportTab,
} from './models/interview-dashboard.model';
import { InterviewDashboardService, PageRequest } from './services/interview-dashboard.service';

// Query-param names. Everything the dashboard shows is reconstructible from the URL, because a language
// switch reloads the whole app (LanguageService.set) and in-memory state would be lost.
const Q = {
  range: 'range',
  from: 'from',
  to: 'to',
  job: 'job',
  committee: 'committee',
  schedule: 'schedule',
  type: 'type',
  scheduleStatus: 'scheduleStatus',
  reportStatus: 'reportStatus',
  decision: 'decision',
  tab: 'tab',
} as const;

const DATE_PRESETS: DatePreset[] = ['all', 'today', 'week', 'month', 'custom'];
const REPORT_TABS: ReportTab[] = ['schedules', 'candidates', 'results', 'issues'];

type TableKey = 'schedules' | 'candidates' | 'results' | 'issues';

@Injectable()
export class InterviewDashboardFacade {
  private store = inject(InterviewDashboardStore);
  private api = inject(InterviewDashboardService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private language = inject(LanguageService);
  private destroyRef = inject(DestroyRef);

  private readonly overview$ = new Subject<InterviewDashboardFilters>();
  private readonly table$ = new Subject<TableKey>();
  private lastFiltersKey: string | null = null;

  init(): void {
    this.store.setCurrentLang(this.language.get());

    this.overview$
      .pipe(
        switchMap((filters) =>
          this.api.getOverview(filters).pipe(
            map((overview) => ({ overview, error: false })),
            catchError(() => of({ overview: null, error: true })),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(({ overview, error }) => {
        this.store.overview.set(overview);
        this.store.overviewError.set(error);
        this.store.overviewLoading.set(false);
        this.ensureTabAvailable();
      });

    this.table$
      .pipe(
        // One pipeline per table would let a slow response overwrite a newer one; switchMap per key keeps
        // only the latest request of the active table.
        switchMap((key) => this.fetchTable(key).pipe(map((result) => ({ key, result })))),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(({ key, result }) => {
        this.patchTable(key, {
          page: result,
          loading: false,
          error: result === null,
          ...(result?.metadata
            ? { pageNumber: result.metadata.currentPage, pageSize: result.metadata.pageSize }
            : {}),
        });
      });

    this.route.queryParamMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((params) => this.applyUrl(params));
  }

  // ---------- filters (all go through the URL) ----------

  setDatePreset(preset: DatePreset): void {
    if (preset === 'custom') {
      const { fromDate, toDate } = this.store.filters();
      this.navigate({ [Q.range]: 'custom', [Q.from]: fromDate ?? toIsoDate(new Date()), [Q.to]: toDate ?? toIsoDate(new Date()) });
      return;
    }
    this.navigate({ [Q.range]: preset === 'all' ? null : preset, [Q.from]: null, [Q.to]: null });
  }

  setCustomRange(from: Date | null, to: Date | null): void {
    if (!from) return;
    this.navigate({ [Q.range]: 'custom', [Q.from]: toIsoDate(from), [Q.to]: toIsoDate(to ?? from) });
  }

  setJob(option: LazySelectOption | null): void {
    this.store.jobOption.set(option);
    // Committee and schedule lists depend on the job, so a job change clears them.
    this.store.committeeOption.set(null);
    this.store.scheduleOption.set(null);
    this.navigate({ [Q.job]: option?.value ?? null, [Q.committee]: null, [Q.schedule]: null });
  }

  setCommittee(option: LazySelectOption | null): void {
    this.store.committeeOption.set(option);
    this.navigate({ [Q.committee]: option?.value ?? null });
  }

  setSchedule(option: LazySelectOption | null): void {
    this.store.scheduleOption.set(option);
    this.navigate({ [Q.schedule]: option?.value ?? null });
  }

  setEnumFilter(key: 'type' | 'scheduleStatus' | 'reportStatus' | 'decision', value: number | null): void {
    this.navigate({ [Q[key]]: value ?? null });
  }

  resetFilters(): void {
    this.store.jobOption.set(null);
    this.store.committeeOption.set(null);
    this.store.scheduleOption.set(null);
    const cleared = Object.fromEntries(Object.values(Q).filter((k) => k !== Q.tab).map((k) => [k, null]));
    this.navigate(cleared);
  }

  refresh(): void {
    this.loadOverview();
    this.loadTable(this.store.activeTab());
  }

  // ---------- detailed reports ----------

  setTab(tab: ReportTab): void {
    this.navigate({ [Q.tab]: tab === 'schedules' ? null : tab });
  }

  onPageChange(key: TableKey, pageNumber: number): void {
    this.patchTable(key, { pageNumber });
    this.loadTable(key);
  }

  onPageSizeChange(key: TableKey, pageSize: number): void {
    this.patchTable(key, { pageNumber: 1, pageSize });
    this.loadTable(key);
  }

  setAttendanceView(view: AttendanceView | null): void {
    this.store.attendanceView.set(view);
    this.onPageChange('candidates', 1);
  }

  setDecisionView(view: DecisionView | null): void {
    this.store.decisionView.set(view);
    this.onPageChange('results', 1);
  }

  setIssueStatusView(status: OperationalIssueStatus | null): void {
    this.store.issueStatusView.set(status);
    this.onPageChange('issues', 1);
  }

  // ---------- lookups for the lazy dropdowns ----------

  readonly searchJobs = (term: string): Observable<LazySelectOption[]> =>
    this.lookup(InterviewDashboardLookupKind.Job, term);

  readonly searchCommittees = (term: string): Observable<LazySelectOption[]> =>
    this.lookup(InterviewDashboardLookupKind.Committee, term, { jobId: this.store.filters().jobId });

  readonly searchSchedules = (term: string): Observable<LazySelectOption[]> =>
    this.lookup(InterviewDashboardLookupKind.Schedule, term, { jobId: this.store.filters().jobId });

  // ---------- PDF export ----------

  // Browser print-to-PDF: renders with the print stylesheet (report header + active filters, no app
  // chrome), which keeps Arabic shaping and RTL exactly as on screen.
  exportPdf(): void {
    if (this.store.printing()) return;
    this.store.printing.set(true);
    document.body.classList.add('interview-dashboard-printing');
    const done = () => {
      document.body.classList.remove('interview-dashboard-printing');
      this.store.printing.set(false);
      window.removeEventListener('afterprint', done);
    };
    window.addEventListener('afterprint', done);
    // Let the print header render before the dialog snapshots the page.
    setTimeout(() => window.print(), 150);
  }

  // ---------- internals ----------

  private applyUrl(params: ParamMap): void {
    const preset = (DATE_PRESETS as string[]).includes(params.get(Q.range) ?? '')
      ? (params.get(Q.range) as DatePreset)
      : 'all';
    const range = resolveRange(preset, params.get(Q.from), params.get(Q.to));

    const filters: InterviewDashboardFilters = {
      fromDate: range.from,
      toDate: range.to,
      jobId: params.get(Q.job) ?? undefined,
      committeeId: params.get(Q.committee) ?? undefined,
      scheduleId: params.get(Q.schedule) ?? undefined,
      interviewType: toNumber(params.get(Q.type)),
      scheduleStatus: toNumber(params.get(Q.scheduleStatus)),
      resultReportStatus: toNumber(params.get(Q.reportStatus)),
      finalDecision: toNumber(params.get(Q.decision)),
    };
    const tab = (REPORT_TABS as string[]).includes(params.get(Q.tab) ?? '')
      ? (params.get(Q.tab) as ReportTab)
      : 'schedules';

    this.store.datePreset.set(range.preset);
    this.store.filters.set(filters);
    this.store.activeTab.set(tab);
    this.restoreLookupLabels(filters);

    const filtersKey = JSON.stringify(filters);
    if (filtersKey !== this.lastFiltersKey) {
      this.lastFiltersKey = filtersKey;
      // New filter set: every cached table page is stale.
      for (const key of REPORT_TABS) this.patchTable(key, { page: null, pageNumber: 1 });
      this.loadOverview();
    }
    this.ensureTabAvailable();
    if (!this.table(tab).page) this.loadTable(tab);
  }

  private ensureTabAvailable(): void {
    const tabs = this.store.availableTabs();
    if (tabs.length > 0 && !tabs.includes(this.store.activeTab())) this.setTab(tabs[0]);
  }

  private loadOverview(): void {
    this.store.overviewLoading.set(true);
    this.store.overviewError.set(false);
    this.overview$.next(this.store.filters());
  }

  private loadTable(key: TableKey): void {
    if (!this.store.availableTabs().includes(key)) return;
    this.patchTable(key, { loading: true, error: false });
    this.table$.next(key);
  }

  private fetchTable(key: TableKey): Observable<PaginatedResult<any> | null> {
    const filters = this.store.filters();
    const state = this.table(key);
    const page: PageRequest = { pageNumber: state.pageNumber, pageSize: state.pageSize };
    const request: Observable<PaginatedResult<any>> =
      key === 'schedules'
        ? this.api.listSchedules(filters, page)
        : key === 'candidates'
          ? this.api.listCandidates(filters, page, this.store.attendanceView())
          : key === 'results'
            ? this.api.listResults(filters, page, this.store.decisionView())
            : this.api.listIssues(filters, page, this.store.issueStatusView());
    return request.pipe(catchError(() => of(null)));
  }

  private table(key: TableKey): TableState<unknown> {
    return this.store[key]() as TableState<unknown>;
  }

  private patchTable(key: TableKey, patch: Partial<TableState<any>>): void {
    (this.store[key] as any).update((state: TableState<unknown>) => ({ ...state, ...patch }));
  }

  // A filter restored from the URL (reload, language switch, shared link) only has an id - fetch its label.
  private restoreLookupLabels(filters: InterviewDashboardFilters): void {
    const restore = (
      id: string | undefined,
      current: LazySelectOption | null,
      kind: InterviewDashboardLookupKind,
      set: (option: LazySelectOption | null) => void,
    ) => {
      if (!id) {
        if (current) set(null);
        return;
      }
      if (current?.value === id) return;
      this.api
        .lookups(kind, '', { id })
        .pipe(catchError(() => of([] as InterviewDashboardLookup[])))
        .subscribe((items) => set(items[0] ? this.toOption(items[0]) : null));
    };

    restore(filters.jobId, this.store.jobOption(), InterviewDashboardLookupKind.Job, (o) => this.store.jobOption.set(o));
    restore(filters.committeeId, this.store.committeeOption(), InterviewDashboardLookupKind.Committee, (o) =>
      this.store.committeeOption.set(o),
    );
    restore(filters.scheduleId, this.store.scheduleOption(), InterviewDashboardLookupKind.Schedule, (o) =>
      this.store.scheduleOption.set(o),
    );
  }

  private lookup(
    kind: InterviewDashboardLookupKind,
    term: string,
    options: { jobId?: string } = {},
  ): Observable<LazySelectOption[]> {
    return this.api.lookups(kind, term, options).pipe(map((items) => items.map((item) => this.toOption(item))));
  }

  private toOption(item: InterviewDashboardLookup): LazySelectOption {
    const label = this.store.localized(item.nameAr, item.nameEn);
    const hint = this.store.localized(item.hintAr, item.hintEn) || null;
    return {
      value: item.id,
      label,
      hint,
      searchText: [item.nameAr, item.nameEn, item.hintAr, item.hintEn].filter(Boolean).join(' '),
    };
  }

  private navigate(queryParams: Record<string, string | number | null>): void {
    this.router.navigate([], { relativeTo: this.route, queryParams, queryParamsHandling: 'merge' });
  }
}

function toNumber(value: string | null): any {
  if (value === null || value === '') return undefined;
  const n = Number(value);
  return Number.isInteger(n) ? n : undefined;
}

// Local calendar date (the appointment slot dates are wall-clock), never toISOString's UTC date.
export function toIsoDate(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

export function resolveRange(
  preset: DatePreset,
  from: string | null,
  to: string | null,
): { preset: DatePreset; from?: string; to?: string } {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  switch (preset) {
    case 'today':
      return { preset, from: toIsoDate(today), to: toIsoDate(today) };
    case 'week': {
      // Week starts on Sunday (the Qatari working week).
      const start = new Date(today);
      start.setDate(today.getDate() - today.getDay());
      const end = new Date(start);
      end.setDate(start.getDate() + 6);
      return { preset, from: toIsoDate(start), to: toIsoDate(end) };
    }
    case 'month':
      return {
        preset,
        from: toIsoDate(new Date(today.getFullYear(), today.getMonth(), 1)),
        to: toIsoDate(new Date(today.getFullYear(), today.getMonth() + 1, 0)),
      };
    case 'custom': {
      const valid = (v: string | null) => (v && /^\d{4}-\d{2}-\d{2}$/.test(v) ? v : undefined);
      const f = valid(from);
      const t = valid(to) ?? f;
      return f ? { preset, from: f, to: t && t >= f ? t : f } : { preset: 'all' };
    }
    default:
      return { preset: 'all' };
  }
}
