import {dropdownOptionsModel, DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';

export interface CountryDto extends Omit<dropdownOptionsModel, 'name' | 'backendName'> {
  nameAr: string;
  nameEn: string;
  isoCode: string;
  codeAlpha: string;
  isActive: boolean;
  code: number;
}
export class CountryVM extends DropdownOptionVM {
  isActive: boolean;
  code: number;
  constructor(dto: CountryDto, isActive?: boolean) {
    super({
      ...dto,
      backendName: dto.nameEn,
      name: dto.nameEn,
      description: '',
      additionalData: {
        ...(dto.additionalData || {}),
        nameAr: dto.nameAr,
        nameEn: dto.nameEn,
        isoCode: dto.isoCode,
        codeAlpha: dto.codeAlpha,
      }
    });
    this.isActive = isActive ?? dto.isActive;
    this.code = dto.code;
  }
}
