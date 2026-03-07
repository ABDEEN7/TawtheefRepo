import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AutoAssignRequest,
  DistributionEmployee,
  DistributionFile,
  DistributionProfilesFilters,
  DistributionResult,
  ManualAssignRequest,
  ReassignRequest,
} from '../models/profile-distribution.models';
import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../../shared/models/dropdown-options.model';

@Injectable({ providedIn: 'root' })
export class ProfileDistributionService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getFiles(filters: DistributionProfilesFilters): Observable<PaginatedResult<DistributionFile>> {
    return this.http.get<PaginatedResult<DistributionFile>>(this.endpoints.distribution.files, filters);
  }

  getEmployees(): Observable<DistributionEmployee[]> {
    return this.http.get<DistributionEmployee[]>(this.endpoints.distribution.employees);
  }

  getTargetEntities(): Observable<dropdownOptionsModel[]> {
    return this.http.get<dropdownOptionsModel[]>(this.endpoints.distribution.targetEntities);
  }

  assignManually(request: ManualAssignRequest): Observable<DistributionResult> {
    return this.http.post<DistributionResult>(this.endpoints.distribution.assignManual, request);
  }

  assignAutomatically(request: AutoAssignRequest): Observable<DistributionResult> {
    return this.http.post<DistributionResult>(this.endpoints.distribution.assignAuto, request);
  }

  reassign(request: ReassignRequest): Observable<DistributionResult> {
    return this.http.post<DistributionResult>(this.endpoints.distribution.reassign, request);
  }
}
