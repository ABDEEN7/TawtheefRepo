import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {HttpService} from '../../../../../core/http/http.service';
import {EndpointsService} from '../../../../../core/http/endpoints.service';
import {PaginatedResult} from '../../../../../core/models/paginated-result.model';
import {CandidateUserDto} from '../models/candidate-user.dto';
import {CandidateUserFilters} from '../models/candidate-user-filters.dto';
import {ProfileApprovalDetail} from '../../profile-managment/approval-list/models/profile-approval.models';
import { ProfileLogDto } from '../../profile-logs/models/profile-log.dto';

@Injectable({ providedIn: 'root' })
export class CandidateUsersService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getCandidateUsers(filters: CandidateUserFilters): Observable<PaginatedResult<CandidateUserDto>> {
    return this.http.get<PaginatedResult<CandidateUserDto>>(this.endpoints.candidateUsers.list, filters);
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
