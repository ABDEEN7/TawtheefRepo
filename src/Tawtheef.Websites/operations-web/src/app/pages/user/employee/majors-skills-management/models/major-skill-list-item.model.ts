import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface MajorSkillListItemModel {
  id: string;
  major: dropdownOptionsModel;
  skill: dropdownOptionsModel;
  isSkillRequired: boolean;
  isActive: boolean;
}
