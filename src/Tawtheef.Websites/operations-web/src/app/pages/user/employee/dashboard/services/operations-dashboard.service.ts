import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {HttpService} from '../../../../../core/http/http.service';
import {OperationsDashboardFilters, OperationsDashboardResponse, TeamPerformanceRow} from '../models/operations-dashboard.model';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';

@Injectable({ providedIn: 'root' })
export class OperationsDashboardService {
  private endpoints = inject(EndpointsService);
  private http = inject(HttpService);

  getDashboard(filters: OperationsDashboardFilters): Observable<OperationsDashboardResponse> {
    return this.http.get<OperationsDashboardResponse>(this.endpoints.operationsDashboard.summary, filters);
  }

  getTeamPerformance(filters: OperationsDashboardFilters): Observable<PaginatedResult<TeamPerformanceRow>> {
    return this.http.get<PaginatedResult<TeamPerformanceRow>>(this.endpoints.operationsDashboard.teamPerformance, filters);
  }
}
