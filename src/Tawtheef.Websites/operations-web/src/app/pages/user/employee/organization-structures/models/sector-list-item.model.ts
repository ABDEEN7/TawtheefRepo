import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface SectorListItemModel extends dropdownOptionsModel {
  isActive?: boolean;
  displayOrder?: number;
}
