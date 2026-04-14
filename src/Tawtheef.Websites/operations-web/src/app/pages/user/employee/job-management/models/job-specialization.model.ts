import { GUID } from '../../../../../shared/types/guid.type';

export interface JobSpecialization {
  id?: GUID;
  jobId?: GUID;
  majorId: GUID;
  subMajorId?: GUID | null;
}
