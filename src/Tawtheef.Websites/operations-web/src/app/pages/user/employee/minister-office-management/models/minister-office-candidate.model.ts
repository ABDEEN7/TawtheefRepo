import { PaginatedRequest } from "../../../../../core/models/paginated-request.model";

export enum MinisterOfficeCandidateStatus {
  NoProfile = 0,
  DraftProfile = 1,
  SubmittedForApproval = 2,
  ReturnedForCorrection = 3,
  ApprovedProfile = 4,
  InvitationsReceived = 5
}

export interface MinisterOfficeCandidateDto {
  id: string;
  qid: string;
  fullNameEn: string;
  fullNameAr: string;
  phoneNumber: string;
  nationalityEn: string;
  nationalityAr: string;
  isFollowUpActive: boolean;
  status: MinisterOfficeCandidateStatus;
  genderEn?: string;
  genderAr?: string;
  targetEntityEn?: string;
  targetEntityAr?: string;
  candidateTypeEn?: string;
  candidateTypeAr?: string;
  isPhoneNumberFromProfile?: boolean;
  createdDate: string;
}

export interface GetMinisterOfficeCandidatesRequest extends PaginatedRequest {
  searchTerm?: string;
  includeInactive?: boolean;
  genderId?: string;
  candidateTypeId?: string;
  targetEntityId?: string;
}

export interface CreateMinisterOfficeCandidateRequest {
  qid: string;
  phoneNumber: string;
  qidExpiryDate: string;
}

export interface UpdateMinisterOfficeCandidatePhoneRequest {
  phoneNumber: string;
}

export interface MinisterOfficeCandidateInvitationDto {
  invitationId: string;
  jobTitleEn: string;
  jobTitleAr: string;
  invitationStatus: string;
  invitedAt: string;
}

export interface MinisterOfficeCandidateAuditLogDto {
  id: string;
  action: string;
  details?: string;
  qid?: string;
  operatorNameEn: string;
  operatorNameAr: string;
  createdDate: string;
}
