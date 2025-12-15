import { FinalApprovalAction } from './profile-approval.models';

export interface FinalizeProfileApprovalRequest {
  action: FinalApprovalAction;
  summary?: string;
  note?: string;
  needsCorrectionItems?: string[];
  rejectionDocument?: File | null;
  exceptionalFile?: File | null;
}
