import {dropdownOptionsModel} from '../../../../../shared/models/dropdown-options.model';

export interface OfficeDto {
  id: string;
  nameAr: string;
  nameEn: string;
  country: dropdownOptionsModel;
  supportedCountries: dropdownOptionsModel[];
  adminEmail: string;
}
