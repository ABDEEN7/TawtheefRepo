import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface ManagementListItemModel extends dropdownOptionsModel {
  isActive?: boolean;
  displayOrder?: number;
  departmentNumber: number;
  sectorId?: string;
  sector?: dropdownOptionsModel | null;
}
