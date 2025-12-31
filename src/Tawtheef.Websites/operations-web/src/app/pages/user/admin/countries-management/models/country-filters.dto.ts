export interface CountryFilters {
  pageNumber: number;
  pageSize: number;
  name?: string;
  isActive?: boolean | null;
}
