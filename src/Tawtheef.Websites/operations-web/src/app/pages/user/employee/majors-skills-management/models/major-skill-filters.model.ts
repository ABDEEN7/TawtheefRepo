export interface MajorSkillFiltersModel {
  search?: string;
  parentMajorId?: string;
  subMajorId?: string;
  skillTypeId?: string;
  isActive?: boolean | null;
  pageNumber: number;
  pageSize: number;
}
