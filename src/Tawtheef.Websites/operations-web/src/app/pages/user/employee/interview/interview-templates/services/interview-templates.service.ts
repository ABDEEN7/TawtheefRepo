import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../../core/models/paginated-result.model';

import { TemplateModel } from '../models/template.model';
import { TemplateVersionDetailsModel, TemplateVersionModel } from '../models/template-version.model';
import { TemplateLookupsModel } from '../models/lookup-option.model';
import { AxisBankOption, CriterionBankOption } from '../models/bank-option.model';
import { CalculationMethod } from '../models/enums';

// Bank axis/criterion dropdowns fetch every active record in one page, same rationale as the
// axes & criteria bank feature: server caps PageSize at 50 (see interview-axes-criteria.service.ts).
const BANK_OPTIONS_PAGE_SIZE = 50;

export interface CreateTemplatePayload {
  titleAr: string;
  titleEn: string | null;
  organizationScopeId: string | null;
  jobTitleId: string | null;
  departmentId: string | null;
  isActive: boolean;
}

export interface UpdateTemplatePayload extends CreateTemplatePayload {
  id: string;
}

export interface CreateTemplateVersionPayload {
  interviewTemplateId: string;
  finalScore: number;
  qualificationScore: number | null;
  calculationMethod: CalculationMethod;
}

export interface UpdateTemplateVersionPayload {
  id: string;
  finalScore: number;
  qualificationScore: number | null;
  calculationMethod: CalculationMethod;
}

export interface AddTemplateVersionAxisPayload {
  interviewTemplateVersionId: string;
  interviewEvaluationAxisId: string;
  maxScore: number;
  qualificationScore: number | null;
  orderNo: number;
}

export interface UpdateTemplateVersionAxisPayload {
  id: string;
  maxScore: number;
  qualificationScore: number | null;
  orderNo: number;
}

export interface AddTemplateVersionCriterionPayload {
  interviewTemplateEvaluationAxisId: string;
  interviewEvaluationCriterionId: string | null;
  nameAr: string | null;
  nameEn: string | null;
  descriptionAr: string | null;
  descriptionEn: string | null;
  maxScore: number;
  isRequired: boolean;
  orderNo: number;
  notes: string | null;
}

export interface UpdateTemplateVersionCriterionPayload {
  id: string;
  interviewEvaluationCriterionId: string | null;
  nameAr: string | null;
  nameEn: string | null;
  descriptionAr: string | null;
  descriptionEn: string | null;
  maxScore: number;
  isRequired: boolean;
  orderNo: number;
  notes: string | null;
}

@Injectable({ providedIn: 'root' })
export class InterviewTemplatesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // ---- Lookups ----
  getLookups(): Observable<TemplateLookupsModel> {
    return this.http.get<TemplateLookupsModel>(this.endpoints.interviewEvaluationTemplate.lookups);
  }

  getAxisOptions(): Observable<PaginatedResult<AxisBankOption>> {
    return this.http.get<PaginatedResult<AxisBankOption>>(this.endpoints.interviewEvaluationBank.axes.list, {
      isActive: true,
      pageNumber: 1,
      pageSize: BANK_OPTIONS_PAGE_SIZE,
    });
  }

  getCriterionOptions(axisId: string): Observable<PaginatedResult<CriterionBankOption>> {
    return this.http.get<PaginatedResult<CriterionBankOption>>(this.endpoints.interviewEvaluationBank.criteria.list, {
      interviewEvaluationAxisId: axisId,
      isActive: true,
      pageNumber: 1,
      pageSize: BANK_OPTIONS_PAGE_SIZE,
    });
  }

  // ---- Templates ----
  listTemplates(): Observable<TemplateModel[]> {
    return this.http.get<TemplateModel[]>(this.endpoints.interviewEvaluationTemplate.templates.list);
  }

  createTemplate(payload: CreateTemplatePayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewEvaluationTemplate.templates.create, payload);
  }

  updateTemplate(payload: UpdateTemplatePayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.templates.update, payload);
  }

  changeTemplateActivation(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.templates.changeActivation, { id, isActive });
  }

  // ---- Versions ----
  listTemplateVersions(interviewTemplateId: string): Observable<TemplateVersionModel[]> {
    return this.http.get<TemplateVersionModel[]>(this.endpoints.interviewEvaluationTemplate.versions.list, {
      interviewTemplateId,
    });
  }

  getTemplateVersionDetails(id: string): Observable<TemplateVersionDetailsModel> {
    return this.http.get<TemplateVersionDetailsModel>(this.endpoints.interviewEvaluationTemplate.versions.details, { id });
  }

  createTemplateVersion(payload: CreateTemplateVersionPayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewEvaluationTemplate.versions.create, payload);
  }

  updateTemplateVersion(payload: UpdateTemplateVersionPayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.versions.update, payload);
  }

  submitTemplateVersion(id: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.versions.submit, { id });
  }

  approveTemplateVersion(id: string, decisionNotes: string | null): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.versions.approve, { id, decisionNotes });
  }

  returnTemplateVersion(id: string, reason: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.versions.return, { id, reason });
  }

  cancelTemplateVersion(id: string, reason: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.versions.cancel, { id, reason });
  }

  // ---- Version axes ----
  addTemplateVersionAxis(payload: AddTemplateVersionAxisPayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewEvaluationTemplate.axes.add, payload);
  }

  updateTemplateVersionAxis(payload: UpdateTemplateVersionAxisPayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.axes.update, payload);
  }

  removeTemplateVersionAxis(id: string): Observable<void> {
    return this.http.delete<void>(this.endpoints.interviewEvaluationTemplate.axes.remove(id));
  }

  // ---- Version axis criteria ----
  addTemplateVersionCriterion(payload: AddTemplateVersionCriterionPayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewEvaluationTemplate.criteria.add, payload);
  }

  updateTemplateVersionCriterion(payload: UpdateTemplateVersionCriterionPayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewEvaluationTemplate.criteria.update, payload);
  }

  removeTemplateVersionCriterion(id: string): Observable<void> {
    return this.http.delete<void>(this.endpoints.interviewEvaluationTemplate.criteria.remove(id));
  }
}
