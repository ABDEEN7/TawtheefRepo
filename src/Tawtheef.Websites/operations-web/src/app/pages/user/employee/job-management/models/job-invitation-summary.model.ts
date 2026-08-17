import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface JobInvitationSummaryModel {
  jobId: string; // Guid
  jobName: string;
  departmentName: string;
  jobCategory: string;
  jobStatus: dropdownOptionsModel;
  invitationCount: number;
  applicantsCount: number;
  refusedCount: number;
  notSeenCount: number;
  readCount: number;
  expiredCount: number;
  cancelledCount: number;
  pendingAttachmentApprovalCount: number;
  returnedAttachmentCount: number;
  previousBatchInvitations: number;
  lastBatchNumber: string | null;
  createDate: string;           // ISO date string
}
