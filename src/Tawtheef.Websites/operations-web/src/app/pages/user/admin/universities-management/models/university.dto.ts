export interface UniversityDto {
  id?: string | null;
  backendName?: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  countryId: string;
  countryNameAr?: string;
  countryNameEn?: string;
  cityId: string;
  cityNameAr?: string;
  cityNameEn?: string;
  webSite?: string;
  phone?: string;
  email?: string;
  code?: string;
  logoEn?: string;
  logoAr?: string;
  originalName?: string;
  isActive: boolean;
}
