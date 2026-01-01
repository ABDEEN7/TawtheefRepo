export interface UniversityDto {
  id?: string | null;
  backendName?: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  countryId: string;
  countryName?: string;
  cityId: string;
  cityName?: string;
  webSite?: string;
  phone?: string;
  email?: string;
  code?: string;
  logoEn?: string;
  logoAr?: string;
  originalName?: string;
  isActive: boolean;
}
