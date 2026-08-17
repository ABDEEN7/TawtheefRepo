import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { DashboardOverview } from '../models/dashboard-overview.model';
import { LatestInvitation } from '../models/dashboard-invitations.model';
import { LatestJob } from '../models/dashboard-jobs.model';
import { OperationsDashboardFilters } from '../models/dashboard-filters.model';
import { TeamPerformanceRow } from '../models/dashboard-employees.model';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';

export type DashboardExportContext = 'Summary' | 'Candidates' | 'Jobs' | 'Employees' | 'Invitations';

@Injectable({ providedIn: 'root' })
export class OperationsDashboardService {
  private endpoints = inject(EndpointsService);
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);

  getOverview(filters: OperationsDashboardFilters): Observable<DashboardOverview> {
    return this.http.get<DashboardOverview>(this.endpoints.operationsDashboard.overview, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getYears(): Observable<number[]> {
    return this.http.get<number[]>(this.endpoints.operationsDashboard.years, undefined, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getLatestJobs(filters: OperationsDashboardFilters): Observable<LatestJob[]> {
    return this.http.get<LatestJob[]>(this.endpoints.operationsDashboard.latestJobs, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getLatestInvitations(filters: OperationsDashboardFilters): Observable<LatestInvitation[]> {
    return this.http.get<LatestInvitation[]>(this.endpoints.operationsDashboard.latestInvitations, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getTeamPerformance(filters: OperationsDashboardFilters): Observable<PaginatedResult<TeamPerformanceRow>> {
    return this.http.get<PaginatedResult<TeamPerformanceRow>>(this.endpoints.operationsDashboard.teamPerformance, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  exportList(context: DashboardExportContext, filters: OperationsDashboardFilters): Observable<HttpResponse<Blob>> {
    return this.httpClient.get(this.endpoints.operationsDashboard.exportList, {
      params: { ...filters, context },
      responseType: 'blob',
      observe: 'response',
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }
}
