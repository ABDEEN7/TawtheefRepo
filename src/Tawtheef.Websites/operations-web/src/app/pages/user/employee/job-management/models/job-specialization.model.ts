import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { GUID } from '../../../../../shared/types/guid.type';

export interface JobSpecialization {
  id?: GUID;
  jobId?: GUID;
  majorId: GUID;
  subMajorId: GUID;
  major?: dropdownOptionsModel;
  subMajor?: dropdownOptionsModel;
}
