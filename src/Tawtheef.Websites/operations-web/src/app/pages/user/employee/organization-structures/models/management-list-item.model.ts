import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface ManagementListItemModel extends dropdownOptionsModel {
  isActive?: boolean;
  displayOrder?: number;
  departmentNumber: string;
  sectorId?: string;
  sector?: dropdownOptionsModel | null;
}
