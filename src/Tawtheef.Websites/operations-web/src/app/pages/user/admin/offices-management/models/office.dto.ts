export interface OfficeSupportedCountryDto {
  countryId: string;
  nameAr: string;
  nameEn: string;
}

export interface OfficeDto {
  id: string;
  nameAr: string;
  nameEn: string;
  countryId: string;
  countryNameAr: string;
  countryNameEn: string;
  supportedCountries: OfficeSupportedCountryDto[];
  adminEmail: string;
}
