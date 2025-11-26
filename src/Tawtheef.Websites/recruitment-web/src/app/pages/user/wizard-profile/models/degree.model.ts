export interface Degree {
  levelId: string;
  level: string;
  majorId?: string;
  major?: string;
  uniId?: string;
  uni?: string;
  year?: number | string;
  gradCountryId?: string;
  gradCountry?: string;
  fileName?: string;
  file?: File | null;
}
