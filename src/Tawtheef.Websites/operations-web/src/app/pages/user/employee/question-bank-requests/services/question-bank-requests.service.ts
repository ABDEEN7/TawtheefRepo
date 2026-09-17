import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { AssignQuestionBankEmployees, CreateQuestionBankRequest, EmployeeLookup, QuestionBankRequestDetails, QuestionBankRequestFilters, QuestionBankRequestListItem } from '../models/question-bank-request.models';

@Injectable({ providedIn: 'root' })
export class QuestionBankRequestsService {
  private readonly http = inject(HttpService); private readonly endpoints = inject(EndpointsService);
  list(filters: QuestionBankRequestFilters): Observable<PaginatedResult<QuestionBankRequestListItem>> { return this.http.get<PaginatedResult<QuestionBankRequestListItem>>(this.endpoints.questionBankRequests.list, filters); }
  create(request: CreateQuestionBankRequest): Observable<string> { return this.http.post<string>(this.endpoints.questionBankRequests.create, request); }
  details(id: string): Observable<QuestionBankRequestDetails> { return this.http.get<QuestionBankRequestDetails>(this.endpoints.questionBankRequests.details(id)); }
  eligibleEmployees(id: string): Observable<EmployeeLookup[]> { return this.http.get<EmployeeLookup[]>(this.endpoints.questionBankRequests.eligibleEmployees(id)); }
  assign(id: string, request: AssignQuestionBankEmployees): Observable<void> { return this.http.post<void>(this.endpoints.questionBankRequests.assignments(id), request); }
  questionBankTypes(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.questionBankTypes); }
  managements(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.managements); }
  jobTitles(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.jobTitles); }
  requestTypes(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.requestTypes); }
  statuses(): Observable<dropdownOptionsModel[]> { return this.http.get<dropdownOptionsModel[]>(this.endpoints.questionBankRequests.lookups.statuses); }
}
