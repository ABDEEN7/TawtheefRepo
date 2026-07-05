export interface OfficeSummaryDto {
  nameAr: string;
  nameEn: string;
  countryNameAr: string;
  countryNameEn: string;
  phoneCountryCode?: string | null;
  phoneNumber?: string | null;
  isActive: boolean;
}
