import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../shared/types/guid.type';

export interface Skill {
  id?: GUID;
  skillId: string;
  skill: dropdownOptionsModel | null | undefined;
  levelId: string;
  level: dropdownOptionsModel | null | undefined;
}
