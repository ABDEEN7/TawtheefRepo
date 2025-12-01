import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';

export interface Skill {
  id?: string | null;
  skillId: string;
  skill: dropdownOptionsModel | null | undefined;
  levelId: string;
  level: dropdownOptionsModel | null | undefined;
}
