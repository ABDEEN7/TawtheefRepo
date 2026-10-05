import { HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../../core/models/paginated-result.model';
import {
  InterviewDashboardCandidateRow,
  InterviewDashboardFilters,
  InterviewDashboardIssueRow,
  InterviewDashboardLookup,
  InterviewDashboardLookupKind,
  InterviewDashboardOverview,
  InterviewDashboardResultRow,
  InterviewDashboardScheduleRow,
} from '../models/interview-dashboard.model';

export interface PageRequest {
  pageNumber: number;
  pageSize: number;
}

// Dashboard requests render their own skeletons, so they skip the global loading overlay
// (same as OperationsDashboardService).
const SKIP_LOADING = { headers: new HttpHeaders({ 'X-Skip-Loading': 'true' }) };

// The date filter and the activity chart work in the viewer's calendar days, but appointment times are stored
// in UTC, so every query carries the browser's offset from UTC in minutes (180 for Qatar).
function withUtcOffset(filters: InterviewDashboardFilters) {
  return { ...filters, utcOffsetMinutes: -new Date().getTimezoneOffset() };
}

@Injectable({ providedIn: 'root' })
export class InterviewDashboardService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getOverview(filters: InterviewDashboardFilters): Observable<InterviewDashboardOverview> {
    return this.http.get<InterviewDashboardOverview>(this.endpoints.interviewDashboard.overview, withUtcOffset(filters), SKIP_LOADING);
  }

  listSchedules(filters: InterviewDashboardFilters, page: PageRequest): Observable<PaginatedResult<InterviewDashboardScheduleRow>> {
    return this.http.get(this.endpoints.interviewDashboard.schedules, { ...withUtcOffset(filters), ...page }, SKIP_LOADING);
  }

  listCandidates(
    filters: InterviewDashboardFilters,
    page: PageRequest,
    attendance: number | null,
  ): Observable<PaginatedResult<InterviewDashboardCandidateRow>> {
    return this.http.get(this.endpoints.interviewDashboard.candidates, { ...withUtcOffset(filters), ...page, attendance }, SKIP_LOADING);
  }

  listResults(
    filters: InterviewDashboardFilters,
    page: PageRequest,
    decision: number | null,
  ): Observable<PaginatedResult<InterviewDashboardResultRow>> {
    return this.http.get(this.endpoints.interviewDashboard.results, { ...withUtcOffset(filters), ...page, decision }, SKIP_LOADING);
  }

  listIssues(
    filters: InterviewDashboardFilters,
    page: PageRequest,
    issueStatus: number | null,
  ): Observable<PaginatedResult<InterviewDashboardIssueRow>> {
    return this.http.get(this.endpoints.interviewDashboard.issues, { ...withUtcOffset(filters), ...page, issueStatus }, SKIP_LOADING);
  }

  lookups(
    kind: InterviewDashboardLookupKind,
    search: string,
    options: { id?: string; jobId?: string } = {},
  ): Observable<InterviewDashboardLookup[]> {
    return this.http.get(this.endpoints.interviewDashboard.lookups, { kind, search, ...options }, SKIP_LOADING);
  }
}
