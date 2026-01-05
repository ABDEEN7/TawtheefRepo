import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface DepartmentListItemModel extends dropdownOptionsModel {
  isActive?: boolean;
  displayOrder?: number;
  managementId?: string;
  management?: dropdownOptionsModel | null;
  sector?: dropdownOptionsModel | null;
}
