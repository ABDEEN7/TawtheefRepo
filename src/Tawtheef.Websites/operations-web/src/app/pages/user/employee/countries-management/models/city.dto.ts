import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';

export interface CityDto extends dropdownOptionsModel {
  countryId: string;
  isActive: boolean;
  code: string;
}

export interface CityFilters {
  pageNumber: number;
  pageSize: number;
  countryId?: string;
  name?: string;
  isActive?: boolean;
}

export class CityVM extends DropdownOptionVM {
  countryId: string;
  isActive: boolean;
  code: string;
  
  constructor(dto: CityDto, isActive?: boolean) {
    super(dto);
    this.countryId = dto.countryId;
    this.isActive = isActive ?? dto.isActive;
    this.code = dto.code;
  }
}
