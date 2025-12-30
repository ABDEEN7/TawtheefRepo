import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface MajorSkillDetailsModel {
  id: string;
  majorId: string;
  major: dropdownOptionsModel;
  skillId: string;
  skill: dropdownOptionsModel;
  isSkillRequired: boolean;
  isActive: boolean;
}
