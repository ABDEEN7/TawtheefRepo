import { GUID } from "../../../shared/types/guid.type";

export interface JobResponsibilityResponse {
  id: GUID;
  jobId: GUID;
  textAr: string;
  textEn: string;
  createdDate: Date;
  lastModifiedDate?: Date;
}