import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../../core/http/endpoints.service';
import { PaginatedResult } from '../../../../../../core/models/paginated-result.model';
import { PaginatedRequest } from '../../../../../../core/models/paginated-request.model';
import { GUID } from '../../../../../../shared/types/guid.type';

import { JobResponse } from '../../../job-management/models/job-response-model';
import { JobQueryFilter } from '../../../job-management/models/job-query-filter.model';

import { CommitteeModel } from '../models/committee.model';
import { CommitteeMemberModel, EligibleMemberModel } from '../models/committee-member.model';
import { CommitteeRole } from '../models/enums';

// The pickers are search-as-you-type: fired on every (debounced) keystroke, so they must not flip the global
// loading overlay each time.
const SKIP_LOADING = { headers: { 'X-Skip-Loading': 'true' } };

// Job search fetches one over-sized page; jobs that already own an active committee are dropped client-side,
// so a few extra rows keep the dropdown from coming back short. Server caps PageSize at 50.
const JOB_SEARCH_PAGE_SIZE = 50;

export interface CommitteeMemberPayload {
  memberUserId: string;
  role: CommitteeRole;
  participatesInEvaluation: boolean;
  canViewCandidates: boolean;
  canAddNotes: boolean;
  canSubmitEvaluation: boolean;
  canViewOtherEvaluations: boolean;
  canViewCommitteeSummary: boolean;
  // Empty means "evaluates all axes" - the backend derives the evaluation scope from this.
  evaluationAxisIds: string[];
}

export interface CreateCommitteePayload {
  jobId: string;
  interviewTemplateId: string;
  nameAr: string;
  nameEn: string | null;
  scopeDescription: string | null;
  notes: string | null;
  members: CommitteeMemberPayload[];
}

export interface UpdateCommitteePayload {
  id: string;
  nameAr: string;
  nameEn: string | null;
  scopeDescription: string | null;
  notes: string | null;
  members: CommitteeMemberPayload[];
}

@Injectable({ providedIn: 'root' })
export class InterviewCommitteesService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  // ---- Lookups ----
  searchEligibleMembers(search: string): Observable<EligibleMemberModel[]> {
    return this.http.get<EligibleMemberModel[]>(
      this.endpoints.interviewCommittee.eligibleMembers,
      { search: search || undefined },
      SKIP_LOADING,
    );
  }

  searchPublishedJobs(statusId: GUID, search: string): Observable<PaginatedResult<JobResponse>> {
    const pagination: PaginatedRequest = {
      pageNumber: 1,
      pageSize: JOB_SEARCH_PAGE_SIZE,
      sortBy: 'createdDate',
      sortDirection: 'desc',
    };
    const filter: JobQueryFilter = { statusId, searchTerm: search || undefined };

    return this.http.post<PaginatedResult<JobResponse>>(
      this.endpoints.job.searchJob,
      { pagination, filter },
      undefined,
      SKIP_LOADING,
    );
  }

  // ---- Committees ----
  listCommittees(): Observable<CommitteeModel[]> {
    return this.http.get<CommitteeModel[]>(this.endpoints.interviewCommittee.list);
  }

  getCommittee(id: string): Observable<CommitteeModel> {
    return this.http.get<CommitteeModel>(this.endpoints.interviewCommittee.details(id));
  }

  listMembers(interviewCommitteeId: string): Observable<CommitteeMemberModel[]> {
    return this.http.get<CommitteeMemberModel[]>(this.endpoints.interviewCommittee.members, { interviewCommitteeId });
  }

  createCommittee(payload: CreateCommitteePayload): Observable<string> {
    return this.http.post<string>(this.endpoints.interviewCommittee.create, payload);
  }

  updateCommittee(payload: UpdateCommitteePayload): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewCommittee.update, payload);
  }

  submitCommittee(id: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewCommittee.submit, { id });
  }

  approveCommittee(id: string, decisionNotes: string | null): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewCommittee.approve, { id, decisionNotes });
  }

  returnCommittee(id: string, reason: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewCommittee.return, { id, reason });
  }

  cancelCommittee(id: string, reason: string): Observable<void> {
    return this.http.put<void>(this.endpoints.interviewCommittee.cancel, { id, reason });
  }
}
