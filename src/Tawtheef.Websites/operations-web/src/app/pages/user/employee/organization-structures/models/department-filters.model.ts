export interface DepartmentFiltersModel {
  search: string;
  sectorId: string;
  managementId: string;
  isActive: boolean | null;
  pageNumber: number;
  pageSize: number;
}
