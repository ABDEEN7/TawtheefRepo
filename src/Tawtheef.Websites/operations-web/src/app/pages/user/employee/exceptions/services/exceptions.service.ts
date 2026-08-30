import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { HttpService } from '../../../../../core/http/http.service';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { GUID } from '../../../../../shared/types/guid.type';
import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import {
  CancelInvitationExceptionResult,
  CreateInvitationExceptionResult,
  ExceptionCandidateLookup,
  ExceptionJobLookup,
  InvitationExceptionDetails,
  InvitationExceptionListItem,
  InvitationExceptionsSummary,
  SendExceptionalInvitationResult
} from '../models/invitation-exception.models';
import {
  CancelInvitationExceptionRequest,
  CreateInvitationExceptionRequest,
  ExceptionJobsRequest,
  ExceptionOrganizationLookupRequest,
  InvitationExceptionsRequest
} from '../models/invitation-exception.requests';

@Injectable({ providedIn: 'root' })
export class ExceptionsService {
  private readonly http = inject(HttpService);
  private readonly httpClient = inject(HttpClient);
  private readonly endpoints = inject(EndpointsService);
  private readonly lookupRequestOptions = {
    headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
  };

  getExceptions(request: InvitationExceptionsRequest): Observable<PaginatedResult<InvitationExceptionListItem>> {
    return this.http.get<PaginatedResult<InvitationExceptionListItem>>(
      this.endpoints.exceptions.list,
      { ...request, search: request.search?.trim() || undefined }
    );
  }

  getSummary(): Observable<InvitationExceptionsSummary> {
    return this.http.get<InvitationExceptionsSummary>(this.endpoints.exceptions.summary);
  }

  getDetails(exceptionId: GUID): Observable<InvitationExceptionDetails> {
    return this.http.get<InvitationExceptionDetails>(this.endpoints.exceptions.details(exceptionId));
  }

  getProof(exceptionId: GUID): Observable<HttpResponse<Blob>> {
    return this.httpClient.get(this.endpoints.exceptions.proof(exceptionId), {
      observe: 'response',
      responseType: 'blob'
    });
  }

  getCandidateByQid(qid: string): Observable<ExceptionCandidateLookup> {
    return this.http.get<ExceptionCandidateLookup>(
      this.endpoints.exceptions.candidateByQid,
      { qid }
    );
  }

  getJobs(request: ExceptionJobsRequest): Observable<PaginatedResult<ExceptionJobLookup>> {
    return this.http.get<PaginatedResult<ExceptionJobLookup>>(
      this.endpoints.exceptions.jobs,
      { ...request, searchTerm: request.searchTerm?.trim() || undefined },
      this.lookupRequestOptions
    );
  }

  getManagements(request: ExceptionOrganizationLookupRequest): Observable<PaginatedResult<dropdownOptionsModel>> {
    return this.http.get<PaginatedResult<dropdownOptionsModel>>(
      this.endpoints.exceptions.managements,
      { ...request, search: request.search?.trim() || undefined },
      this.lookupRequestOptions
    );
  }

  getDepartments(request: ExceptionOrganizationLookupRequest): Observable<PaginatedResult<dropdownOptionsModel>> {
    return this.http.get<PaginatedResult<dropdownOptionsModel>>(
      this.endpoints.exceptions.departments,
      { ...request, search: request.search?.trim() || undefined },
      this.lookupRequestOptions
    );
  }

  createException(request: CreateInvitationExceptionRequest): Observable<CreateInvitationExceptionResult> {
    const formData = new FormData();
    formData.append('Qid', request.qid);
    formData.append('JobId', request.jobId);
    formData.append('Reason', request.reason);
    formData.append('Proof', request.proof);

    return this.http.post<CreateInvitationExceptionResult>(this.endpoints.exceptions.create, formData);
  }

  sendInvitation(exceptionId: GUID): Observable<SendExceptionalInvitationResult> {
    return this.http.post<SendExceptionalInvitationResult>(
      this.endpoints.exceptions.sendInvitation(exceptionId),
      {}
    );
  }

  cancelException(
    exceptionId: GUID,
    reason: string
  ): Observable<CancelInvitationExceptionResult> {
    const request: CancelInvitationExceptionRequest = { reason };
    return this.http.post<CancelInvitationExceptionResult>(
      this.endpoints.exceptions.cancel(exceptionId),
      request
    );
  }
}
