import {GUID} from '../../../../shared/types/guid.type';

export interface JobConditionResponse {
  id: GUID;
  jobId: string;
  text: string;
  createdDate: Date;
  lastModifiedDate?: Date;
}
