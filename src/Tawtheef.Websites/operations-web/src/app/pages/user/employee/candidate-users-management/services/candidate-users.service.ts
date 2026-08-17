import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {CandidateUserDto} from '../models/candidate-user.dto';
import {CandidateUserFilters} from '../models/candidate-user-filters.dto';
import {ProfileApprovalDetail} from '../../profile-managment/approval-list/models/profile-approval.models';
import { ProfileLogDto } from '../../profile-logs/models/profile-log.dto';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class CandidateUsersService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);
  private httpClient = inject(HttpClient);

  getCandidateUsers(filters: CandidateUserFilters): Observable<PaginatedResult<CandidateUserDto>> {
    return this.http.get<PaginatedResult<CandidateUserDto>>(this.endpoints.candidateUsers.list, filters);
  }

  exportCandidateUsers(filters: CandidateUserFilters): Observable<HttpResponse<Blob>> {
    let params = new HttpParams().set('scope', filters.scope);
    if (filters.name) params = params.set('name', filters.name);
    if (filters.email) params = params.set('email', filters.email);
    if (filters.qid) params = params.set('qid', filters.qid);
    if (filters.mobileNumber) params = params.set('mobileNumber', filters.mobileNumber);
    if (filters.profileStatus) params = params.set('profileStatus', filters.profileStatus);
    if (filters.year) params = params.set('year', filters.year);
    return this.httpClient.get(this.endpoints.candidateUsers.export, {
      params,
      responseType: 'blob',
      observe: 'response',
      headers: new HttpHeaders({ 'X-Skip-Loading': 'true' })
    });
  }

  updateBlockStatus(userId: string, isBlocked: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.candidateUsers.blockStatus(userId), { isBlocked });
  }

  getCandidateProfile(userId: string): Observable<ProfileApprovalDetail> {
    return this.http.get<ProfileApprovalDetail>(this.endpoints.candidateUsers.profile(userId));
  }

  getCandidateProfileLogs(userId: string, pageNumber = 1, pageSize = 20): Observable<PaginatedResult<ProfileLogDto>> {
    return this.http.get<PaginatedResult<ProfileLogDto>>(this.endpoints.candidateUsers.profileLogs(userId), {
      pageNumber,
      pageSize
    });
  }
}
