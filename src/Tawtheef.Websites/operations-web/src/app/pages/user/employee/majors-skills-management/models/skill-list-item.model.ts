import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface SkillListItemModel extends dropdownOptionsModel {
  isActive?: boolean;
  skillTypeId?: string;
  skillTypeName?: string;
  additionalData?: any;
}
