import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../../core/http/http.service';
import { OperationsDashboardFilters, OperationsDashboardResponse } from '../models/operations-dashboard.model';

@Injectable({ providedIn: 'root' })
export class OperationsDashboardService {
  private endpoints = inject(EndpointsService);
  private http = inject(HttpService);

  getDashboard(filters: OperationsDashboardFilters): Observable<OperationsDashboardResponse> {
    return this.http.get<OperationsDashboardResponse>(this.endpoints.operationsDashboard.summary, filters);
  }
}
