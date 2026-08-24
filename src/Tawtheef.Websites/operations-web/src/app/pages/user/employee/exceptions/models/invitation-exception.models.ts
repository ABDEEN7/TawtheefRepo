import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';
import { GUID } from '../../../../../shared/types/guid.type';

export enum InvitationExceptionStatus {
  ReadyToSend = 1,
  InvitationSent = 2,
  Applied = 3,
  Expired = 4,
  Cancelled = 5
}

export interface InvitationExceptionListItem {
  exceptionId: GUID;
  applicantId: GUID;
  candidateName: string;
  gender : string;
  qid: string;
  jobId: GUID;
  jobNumber: string;
  jobTitle: string;
  reason: string;
  status: InvitationExceptionStatus;
  invitationId: GUID | null;
  invitationStatusId: GUID | null;
  invitationStatusName: string | null;
  createdDate: string;
  cancellationReason: string | null;
  hasProof: boolean;
}

export interface InvitationExceptionsSummary {
  total: number;
  readyToSend: number;
  invitationSent: number;
  applied: number;
  expired: number;
  cancelled: number;
}

export interface InvitationExceptionDetails {
  exceptionId: GUID;
  candidate: InvitationExceptionCandidate;
  job: InvitationExceptionJob;
  reason: string;
  cancellationReason: string | null;
  status: InvitationExceptionStatus;
  createdDate: string;
  updatedDate: string | null;
  invitationId: GUID | null;
  invitationStatusId: GUID | null;
  invitationStatusName: string | null;
  proof: InvitationExceptionProof | null;
}

export interface InvitationExceptionCandidate {
  applicantId: GUID;
  candidateName: string;
  qid: string;
}

export interface InvitationExceptionJob {
  jobId: GUID;
  jobNumber: string;
  jobTitle: string;
}

export interface InvitationExceptionProof {
  resourceId: GUID;
  fileName: string;
  size: number;
  contentType: string;
}

export interface ExceptionCandidateLookup {
  applicantId: GUID;
  profileId: GUID;
  qid: string;
  candidateName: string;
  gender: string;
  profileStatus: ProfileStatusNumber;
}

export interface ExceptionJobLookup {
  jobId: GUID;
  jobNumber: string;
  jobTitle: string;
  closingDate: string;
  jobStatusId: GUID;
  status: string;
}

export interface CreateInvitationExceptionResult {
  exceptionId: GUID;
  status: InvitationExceptionStatus;
  applicantId: GUID;
  jobId: GUID;
}

export interface SendExceptionalInvitationResult {
  exceptionId: GUID;
  invitationId: GUID;
  exceptionStatus: InvitationExceptionStatus;
  invitationStatusId: GUID;
  expiresOn: string;
}

export interface CancelInvitationExceptionResult {
  exceptionId: GUID;
  exceptionStatus: InvitationExceptionStatus;
  invitationId: GUID | null;
  invitationStatusId: GUID | null;
}
