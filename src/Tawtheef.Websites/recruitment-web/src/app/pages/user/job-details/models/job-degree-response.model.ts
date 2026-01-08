import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';

export interface JobDegreeResponse {
  id: GUID;
  jobId: GUID;
  degreeId: GUID;
  degree: dropdownOptionsModel;
  createdDate: Date;
  lastModifiedDate?: Date;
}
