import { convertToParamMap, ActivatedRoute, Router } from '@angular/router';
import { TestBed } from '@angular/core/testing';
import { BehaviorSubject, of } from 'rxjs';

import { AuthService } from '../../../../../core/auth/auth.service';
import { LanguageService } from '../../../../../core/services/language.service';
import { InterviewDashboardFacade, resolveRange, toIsoDate } from './interview-dashboard.facade';
import { InterviewDashboardStore } from './interview-dashboard.store';
import { InterviewDashboardOverview } from './models/interview-dashboard.model';
import { InterviewDashboardService } from './services/interview-dashboard.service';

describe('Interview dashboard date ranges', () => {
  beforeEach(() => jasmine.clock().install());
  afterEach(() => jasmine.clock().uninstall());

  it('formats local calendar dates, not UTC dates', () => {
    // 00:30 local on the 1st is still the previous day in UTC for zones east of UTC (e.g. Qatar).
    expect(toIsoDate(new Date(2026, 8, 1, 0, 30))).toBe('2026-09-01');
  });

  it('resolves today / this week (Sunday start) / this month', () => {
    jasmine.clock().mockDate(new Date(2026, 8, 24, 15, 0)); // Thursday 24 Sep 2026

    expect(resolveRange('today', null, null)).toEqual({ preset: 'today', from: '2026-09-24', to: '2026-09-24' });
    expect(resolveRange('week', null, null)).toEqual({ preset: 'week', from: '2026-09-20', to: '2026-09-26' });
    expect(resolveRange('month', null, null)).toEqual({ preset: 'month', from: '2026-09-01', to: '2026-09-30' });
  });

  it('keeps a valid custom range and repairs a reversed or half range', () => {
    expect(resolveRange('custom', '2026-09-01', '2026-09-10')).toEqual({ preset: 'custom', from: '2026-09-01', to: '2026-09-10' });
    expect(resolveRange('custom', '2026-09-10', '2026-09-01')).toEqual({ preset: 'custom', from: '2026-09-10', to: '2026-09-10' });
    expect(resolveRange('custom', '2026-09-10', null)).toEqual({ preset: 'custom', from: '2026-09-10', to: '2026-09-10' });
  });

  it('falls back to all dates for a malformed custom range', () => {
    expect(resolveRange('custom', 'not-a-date', '2026-09-10')).toEqual({ preset: 'all' });
    expect(resolveRange('all', '2026-09-01', '2026-09-10')).toEqual({ preset: 'all' });
  });
});

describe('InterviewDashboardFacade URL state', () => {
  const overview = (execution: boolean, results: boolean): InterviewDashboardOverview => ({
    sections: { templates: false, committees: false, execution, results },
    templates: null,
    committees: null,
    schedules: null,
    candidates: null,
    issues: null,
    results: null,
    activity: null,
  });

  let params$: BehaviorSubject<ReturnType<typeof convertToParamMap>>;
  let api: jasmine.SpyObj<InterviewDashboardService>;
  let router: jasmine.SpyObj<Router>;
  let store: InterviewDashboardStore;
  let facade: InterviewDashboardFacade;

  const setup = (query: Record<string, string>, sections = overview(true, true), permissions = true) => {
    params$ = new BehaviorSubject(convertToParamMap(query));
    api = jasmine.createSpyObj<InterviewDashboardService>('InterviewDashboardService', [
      'getOverview',
      'listSchedules',
      'listCandidates',
      'listResults',
      'listIssues',
      'lookups',
    ]);
    const emptyPage = of({ items: [], metadata: { totalCount: 0, pageSize: 10, currentPage: 1 } as any });
    api.getOverview.and.returnValue(of(sections));
    api.listSchedules.and.returnValue(emptyPage);
    api.listCandidates.and.returnValue(emptyPage);
    api.listResults.and.returnValue(emptyPage);
    api.listIssues.and.returnValue(emptyPage);
    api.lookups.and.returnValue(of([{ id: 'job-1', nameAr: 'وظيفة', nameEn: 'Job', hintAr: null, hintEn: null }]));
    router = jasmine.createSpyObj<Router>('Router', ['navigate']);

    TestBed.configureTestingModule({
      providers: [
        InterviewDashboardStore,
        InterviewDashboardFacade,
        { provide: InterviewDashboardService, useValue: api },
        { provide: Router, useValue: router },
        { provide: ActivatedRoute, useValue: { queryParamMap: params$ } },
        { provide: LanguageService, useValue: { get: () => 'en' } },
        { provide: AuthService, useValue: { hasPermission: () => permissions } },
      ],
    });
    store = TestBed.inject(InterviewDashboardStore);
    facade = TestBed.inject(InterviewDashboardFacade);
    facade.init();
  };

  it('restores filters, tab and the job label from the URL', () => {
    setup({ job: 'job-1', type: '2', decision: '3', tab: 'results', range: 'custom', from: '2026-09-01', to: '2026-09-05' });

    expect(store.filters()).toEqual(
      jasmine.objectContaining({ jobId: 'job-1', interviewType: 2, finalDecision: 3, fromDate: '2026-09-01', toDate: '2026-09-05' }),
    );
    expect(store.activeTab()).toBe('results');
    expect(store.jobOption()?.label).toBe('Job');
    expect(api.getOverview).toHaveBeenCalledTimes(1);
    expect(api.listResults).toHaveBeenCalled();
  });

  it('ignores non-numeric enum params instead of sending them to the API', () => {
    setup({ type: 'abc' });

    expect(store.filters().interviewType).toBeUndefined();
  });

  it('moves off a tab the server says the user may not see', () => {
    setup({ tab: 'results' }, overview(true, false), false);

    expect(router.navigate).toHaveBeenCalledWith([], jasmine.objectContaining({ queryParams: { tab: null } }));
    expect(api.listResults).not.toHaveBeenCalled();
  });

  it('clears committee and schedule when the job changes', () => {
    setup({ job: 'job-1', committee: 'c-1', schedule: 's-1' });

    facade.setJob({ value: 'job-2', label: 'Other job' });

    expect(router.navigate).toHaveBeenCalledWith(
      [],
      jasmine.objectContaining({ queryParams: { job: 'job-2', committee: null, schedule: null } }),
    );
  });

  it('reloads the overview only when the filters change, not on a tab switch', () => {
    setup({});
    params$.next(convertToParamMap({ tab: 'issues' }));
    expect(api.getOverview).toHaveBeenCalledTimes(1);

    params$.next(convertToParamMap({ tab: 'issues', type: '1' }));
    expect(api.getOverview).toHaveBeenCalledTimes(2);
  });
});
