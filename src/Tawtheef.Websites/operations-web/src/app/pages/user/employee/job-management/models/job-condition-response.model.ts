import { GUID } from "../../../../../shared/types/guid.type";

export interface JobConditionResponse{
  id: GUID;
  jobId: string;
  textAr: string;
  textEn: string;
  createdDate: Date;
  lastModifiedDate?: Date;
}
