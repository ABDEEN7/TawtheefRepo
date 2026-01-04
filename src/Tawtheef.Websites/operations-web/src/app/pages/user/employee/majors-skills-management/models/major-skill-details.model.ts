import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';
import {majorDetails} from './major.details';

export interface MajorSkillDetailsModel {
  id: string;
  majorId: string;
  major: majorDetails;
  skillId: string;
  skill: dropdownOptionsModel;
  isSkillRequired: boolean;
  isActive: boolean;
}
