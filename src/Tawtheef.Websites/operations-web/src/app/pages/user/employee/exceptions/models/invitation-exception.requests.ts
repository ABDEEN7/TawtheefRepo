import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';
import { GUID } from '../../../../../shared/types/guid.type';
import { InvitationExceptionStatus } from './invitation-exception.models';

export interface InvitationExceptionsRequest extends PaginatedRequest {
  search?: string;
  status?: InvitationExceptionStatus;
}

export interface ExceptionJobsRequest extends PaginatedRequest {
  searchTerm?: string;
}

export interface CreateInvitationExceptionRequest {
  qid: string;
  jobId: GUID;
  reason: string;
  proof: File;
}

export interface CancelInvitationExceptionRequest {
  reason: string;
}
