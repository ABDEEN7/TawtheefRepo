import {DropdownOptionVM} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';

export interface JobSpecializationResponse {
  id: GUID;
  jobId: GUID;
  majorId: GUID;
  subMajorId: GUID;
  major: DropdownOptionVM;
  subMajor: DropdownOptionVM;
}
