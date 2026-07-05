import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface OfficeDto {
  id: string;
  nameAr: string;
  nameEn: string;
  country: dropdownOptionsModel;
  supportedCountries: dropdownOptionsModel[];
  isActive: boolean;
  adminNameAr?: string;
  adminNameEn?: string;
  adminEmail: string;
  phoneCountryCode?: string;
  phoneNumber?: string;
}
