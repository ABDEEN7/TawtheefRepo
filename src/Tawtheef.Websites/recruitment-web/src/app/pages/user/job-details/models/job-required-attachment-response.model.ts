import {GUID} from '../../../../shared/types/guid.type';

export interface JobRequiredAttachmentResponse {
  id: GUID;
  jobId: string;
  title: string;
  isMandatory: boolean;
  createdDate: Date;
  lastModifiedDate?: Date;
  
  attachmentId?: GUID;
  resourceId?: GUID;
  resourceUrl?: string;
  resourceName?: string;
  isApproved?: boolean;
  isReturned?: boolean;
  reviewNote?: string;
}
