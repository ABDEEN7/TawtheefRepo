import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { CreateQuestionBankRequest, QuestionBankRequestFilters, QuestionBankRequestListItem } from '../models/question-bank-request.models';

@Injectable({ providedIn: 'root' })
export class QuestionBankRequestsService {
  private readonly http = inject(HttpService); private readonly endpoints = inject(EndpointsService);
  list(filters: QuestionBankRequestFilters): Observable<PaginatedResult<QuestionBankRequestListItem>> { return this.http.get<PaginatedResult<QuestionBankRequestListItem>>(this.endpoints.questionBankRequests.list, filters); }
  create(request: CreateQuestionBankRequest): Observable<string> { return this.http.post<string>(this.endpoints.questionBankRequests.create, request); }
  questionBankTypes(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.questionBankTypes); }
  managements(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.managements); }
  jobTitles(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.jobTitles); }
  requestTypes(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.requestTypes); }
  statuses(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.statuses); }
}
