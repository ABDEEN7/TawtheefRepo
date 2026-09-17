import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../../core/models/paginated-result.model';

import { AxisModel } from '../models/axis.model';
import { CriterionModel } from '../models/criterion.model';
import { AxisFiltersModel } from '../models/axis-filters.model';
import { CriterionFiltersModel } from '../models/criterion-filters.model';

// Dropdown lookup (axis filter, create/edit criterion axis select) fetches every axis in one page.
// Server caps PageSize at 50 (Tawtheef.Application.Common.Models.Pagination.PaginatedRequest);
// if the number of axes can exceed that, this needs a dedicated non-paginated lookup endpoint.
const AXIS_OPTIONS_PAGE_SIZE = 50;

@Injectable({ providedIn: 'root' })
export class InterviewAxesCriteriaService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // Server-side search, status filter and pagination.
  getAxes(filters: Pick<AxisFiltersModel, 'search' | 'isActive' | 'pageNumber' | 'pageSize'>): Observable<PaginatedResult<AxisModel>> {
    return this.http.get<PaginatedResult<AxisModel>>(this.endpoints.interviewEvaluationBank.axes.list, {
      search: filters.search || undefined,
      isActive: filters.isActive,
      pageNumber: filters.pageNumber,
      pageSize: filters.pageSize,
    });
  }

  // Full axes list (no search/status filter) for dropdowns.
  getAxisOptions(): Observable<PaginatedResult<AxisModel>> {
    return this.http.get<PaginatedResult<AxisModel>>(this.endpoints.interviewEvaluationBank.axes.list, {
      pageNumber: 1,
      pageSize: AXIS_OPTIONS_PAGE_SIZE,
    });
  }

  createAxis(payload: Partial<AxisModel>): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewEvaluationBank.axes.create, payload);
  }

  updateAxis(id: string, payload: Partial<AxisModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationBank.axes.update, { id, ...payload });
  }

  changeAxisActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationBank.axes.changeActivation, { id, isActive });
  }

  // Server-side search, status filter and pagination for the given axis's criteria.
  getCriteria(filters: Pick<CriterionFiltersModel, 'axisId' | 'search' | 'isActive' | 'pageNumber' | 'pageSize'>): Observable<PaginatedResult<CriterionModel>> {
    return this.http.get<PaginatedResult<CriterionModel>>(this.endpoints.interviewEvaluationBank.criteria.list, {
      interviewEvaluationAxisId: filters.axisId,
      search: filters.search || undefined,
      isActive: filters.isActive,
      pageNumber: filters.pageNumber,
      pageSize: filters.pageSize,
    });
  }

  createCriterion(payload: Partial<CriterionModel>): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewEvaluationBank.criteria.create, payload);
  }

  updateCriterion(id: string, payload: Partial<CriterionModel>): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationBank.criteria.update, { id, ...payload });
  }

  changeCriterionActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationBank.criteria.changeActivation, { id, isActive });
  }
}
