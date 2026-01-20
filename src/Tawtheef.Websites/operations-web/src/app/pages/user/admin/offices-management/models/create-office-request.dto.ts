export interface CreateOfficeRequest {
  nameAr: string;
  nameEn: string;
  countryId: string;
  supportedCountryIds: string[];
  adminNameAr: string;
  adminNameEn: string;
  adminEmail: string;
  phoneCountryCode: string;
  phoneNumber: string;
}
