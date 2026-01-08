import {GUID} from '../../../../shared/types/guid.type';

export interface JobResponsibilityResponse {
  id: GUID;
  jobId: GUID;
  text: string;
  createdDate: Date;
  lastModifiedDate?: Date;
}
