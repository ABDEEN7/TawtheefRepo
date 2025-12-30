import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface MajorListItemModel extends dropdownOptionsModel {
  parentId?: string | null;
  isActive?: boolean;
}
