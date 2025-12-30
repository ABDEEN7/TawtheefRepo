import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface SkillListItemModel extends dropdownOptionsModel {
  isActive?: boolean;
  skillTypeId?: GUID;
  skillType?: dropdownOptionsModel;
  additionalData?: any;
}
