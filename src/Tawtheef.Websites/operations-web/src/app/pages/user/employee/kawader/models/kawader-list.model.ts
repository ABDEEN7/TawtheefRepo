import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';

export interface KawaderQidDto {
  qid: string;
  fullName?: string;
  email?: string;
  phoneNumber?: string;
  isInvited: boolean;
  invitedAt?: string;
  createdDate: string;
}

export interface GetKawaderQidsRequest extends PaginatedRequest {
  searchTerm?: string;
}
