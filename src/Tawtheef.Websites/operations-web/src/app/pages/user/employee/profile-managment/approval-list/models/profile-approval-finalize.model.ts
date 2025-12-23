import { FinalApprovalAction } from './profile-approval.models';

export interface FinalizeProfileApprovalRequest {
  summary?: string | null;
  note?: string | null;
  exceptionalFile?: File | null;
}
