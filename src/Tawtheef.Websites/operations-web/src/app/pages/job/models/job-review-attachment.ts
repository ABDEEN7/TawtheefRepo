import { GUID } from "../../../shared/types/guid.type";

export interface JobReviewAttachment {
  id?: GUID;
  fileName: string;
  file?: File;
  originalFileName?: string;
}