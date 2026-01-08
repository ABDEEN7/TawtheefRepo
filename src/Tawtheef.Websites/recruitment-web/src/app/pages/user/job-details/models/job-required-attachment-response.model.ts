import {GUID} from '../../../../shared/types/guid.type';

export interface JobRequiredAttachmentResponse {
  id: GUID;
  jobId: string;
  title: string;
  isMandatory: boolean;
  createdDate: Date;
  lastModifiedDate?: Date;
}
