import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import {
  CandidateStatusSummary,
  CandidateTypeSummary,
  DashboardOverview,
  EmployeeIndicators,
  EmployeeReviewOutcomes,
  JobsSummary,
  LatestJob,
  OperationsDashboardFilters,
  OperationsDashboardResponse,
  TeamPerformanceRow
} from '../models/operations-dashboard.model';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { HttpHeaders } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class OperationsDashboardService {
  private endpoints = inject(EndpointsService);
  private http = inject(HttpService);

  getDashboard(filters: OperationsDashboardFilters): Observable<OperationsDashboardResponse> {
    return this.http.get<OperationsDashboardResponse>(this.endpoints.operationsDashboard.summary, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getOverview(filters: OperationsDashboardFilters): Observable<DashboardOverview> {
    return this.http.get<DashboardOverview>(this.endpoints.operationsDashboard.overview, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getCandidateStatus(filters: OperationsDashboardFilters): Observable<CandidateStatusSummary> {
    return this.http.get<CandidateStatusSummary>(this.endpoints.operationsDashboard.candidateStatus, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getCandidateTypes(filters: OperationsDashboardFilters): Observable<CandidateTypeSummary> {
    return this.http.get<CandidateTypeSummary>(this.endpoints.operationsDashboard.candidateTypes, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getJobsSummary(filters: OperationsDashboardFilters): Observable<JobsSummary> {
    return this.http.get<JobsSummary>(this.endpoints.operationsDashboard.jobsSummary, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getLatestJobs(filters: OperationsDashboardFilters): Observable<LatestJob[]> {
    return this.http.get<LatestJob[]>(this.endpoints.operationsDashboard.latestJobs, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getEmployeeIndicators(filters: OperationsDashboardFilters): Observable<EmployeeIndicators> {
    return this.http.get<EmployeeIndicators>(this.endpoints.operationsDashboard.employeeIndicators, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getEmployeeReviewOutcomes(filters: OperationsDashboardFilters): Observable<EmployeeReviewOutcomes> {
    return this.http.get<EmployeeReviewOutcomes>(this.endpoints.operationsDashboard.employeeReviewOutcomes, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  getTeamPerformance(filters: OperationsDashboardFilters): Observable<PaginatedResult<TeamPerformanceRow>> {
    return this.http.get<PaginatedResult<TeamPerformanceRow>>(this.endpoints.operationsDashboard.teamPerformance, filters, {
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }
}
