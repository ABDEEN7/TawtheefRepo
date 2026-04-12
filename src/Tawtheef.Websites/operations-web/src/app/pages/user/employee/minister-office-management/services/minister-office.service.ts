import { inject, Injectable } from '@angular/core';
import { HttpService } from '../../../../../core/http/http.service';
import { EndpointsService } from '../../../../../core/http/endpoints.service';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../../../../../core/models/paginated-result.model';
import { 
  CreateMinisterOfficeCandidateRequest, 
  GetMinisterOfficeCandidatesRequest, 
  MinisterOfficeCandidateAuditLogDto, 
  MinisterOfficeCandidateDto, 
  MinisterOfficeCandidateInvitationDto, 
  UpdateMinisterOfficeCandidatePhoneRequest 
} from '../models/minister-office-candidate.model';

@Injectable({ providedIn: 'root' })
export class MinisterOfficeService {
  private http = inject(HttpService);
  private endpoints = inject(EndpointsService);

  getCandidates(request: GetMinisterOfficeCandidatesRequest): Observable<PaginatedResult<MinisterOfficeCandidateDto>> {
    return this.http.get<PaginatedResult<MinisterOfficeCandidateDto>>(this.endpoints.ministerOffice.candidates, request);
  }

  createCandidate(request: CreateMinisterOfficeCandidateRequest): Observable<MinisterOfficeCandidateDto> {
    return this.http.post<MinisterOfficeCandidateDto>(this.endpoints.ministerOffice.candidates, request);
  }

  updatePhone(id: string, request: UpdateMinisterOfficeCandidatePhoneRequest): Observable<void> {
    return this.http.put<void>(this.endpoints.ministerOffice.phone(id), request);
  }

  updateFollowUpStatus(id: string, isActive: boolean): Observable<void> {
    return this.http.put<void>(this.endpoints.ministerOffice.followUpStatus(id), { isActive });
  }

  getInvitations(id: string): Observable<MinisterOfficeCandidateInvitationDto[]> {
    return this.http.get<MinisterOfficeCandidateInvitationDto[]>(this.endpoints.ministerOffice.invitations(id));
  }

  getAuditLog(id: string, request: any): Observable<PaginatedResult<MinisterOfficeCandidateAuditLogDto>> {
    return this.http.get<PaginatedResult<MinisterOfficeCandidateAuditLogDto>>(this.endpoints.ministerOffice.auditLog(id), request);
  }

  getGenders(): Observable<any[]> {
    return this.http.get<any[]>(this.endpoints.ministerOffice.genders);
  }

  getTargetEntities(): Observable<any[]> {
    return this.http.get<any[]>(this.endpoints.ministerOffice.targetEntities);
  }

  getCandidateTypes(): Observable<any[]> {
    return this.http.get<any[]>(this.endpoints.ministerOffice.candidateTypes);
  }
}
