export interface LocationDto {
  id: string;
  nameAr: string;
  nameEn: string | null;
  locationLink: string;
  notes: string | null;
}

export interface LocationFilters {
  pageNumber: number;
  pageSize: number;
  search?: string;
}

export interface SaveLocationRequest {
  nameAr: string;
  nameEn: string | null;
  locationLink: string;
  notes: string | null;
}
