import { GUID } from "../../../shared/types/guid.type";

export interface JobReviewAttachmentResponse {
  id: GUID;
  attachmentId: GUID;
  fileName: string;
  url?: string; 
}