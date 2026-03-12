export interface UniversityFormPayload {
  id?: string | null;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  countryId: string;
  cityId: string;
  webSite?: string;
  phone?: string;
  email?: string;
  code?: string;
  originalName?: string;
  isActive: boolean;
  logoArFile?: File | null;
  logoEnFile?: File | null;
}
