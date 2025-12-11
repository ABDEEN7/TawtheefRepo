import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  AutoAssignRequest,
  DistributionEmployee,
  DistributionFile,
  DistributionResult,
  ManualAssignRequest,
  ReassignRequest,
} from '../models/profile-distribution.models';
import {HttpService} from '../../../../core/http/http.service';
import {EndpointsService} from '../../../../core/http/endpoints.service';

@Injectable({ providedIn: 'root' })
export class ProfileDistributionService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getFiles(): Observable<DistributionFile[]> {
    return this.http.get<DistributionFile[]>(this.endpoints.distribution.files);
  }

  getEmployees(): Observable<DistributionEmployee[]> {
    return this.http.get<DistributionEmployee[]>(this.endpoints.distribution.employees);
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
