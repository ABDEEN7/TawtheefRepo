import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface Skill {
  id?: GUID;
  skillId: string;
  skill?: DropdownOptionVM | null;
  levelId: string;
  level?: DropdownOptionVM | null;
}
