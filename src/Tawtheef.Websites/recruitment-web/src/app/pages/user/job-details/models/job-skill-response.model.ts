import {dropdownOptionsModel, DropdownOptionVM} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';

export interface JobSkillResponse {
  id: GUID;
  jobId: GUID;
  skillId: GUID;
  skill: DropdownOptionVM;
  showToApplicants: boolean;
  createdDate: Date;
  lastModifiedDate?: Date;
}
