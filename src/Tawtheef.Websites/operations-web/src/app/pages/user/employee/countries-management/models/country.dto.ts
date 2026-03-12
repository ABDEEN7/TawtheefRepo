import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';

export interface CountryDto extends dropdownOptionsModel {
  isActive: boolean;
  code: string;
}
export class CountryVM extends DropdownOptionVM {
  isActive: boolean;
  code: string;
  constructor(dto: CountryDto, isActive?: boolean) {
    super(dto);
    this.isActive = isActive ?? dto.isActive;
    this.code = dto.code;
  }
}
