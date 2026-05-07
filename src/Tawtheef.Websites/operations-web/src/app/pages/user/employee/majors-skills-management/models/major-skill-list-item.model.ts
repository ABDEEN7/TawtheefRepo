import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';
import { majorDetails } from './major.details';

export interface MajorSkillListItemModel {
  id: string;
  major: majorDetails;
  skill: dropdownOptionsModel;
  isActive: boolean;
}

