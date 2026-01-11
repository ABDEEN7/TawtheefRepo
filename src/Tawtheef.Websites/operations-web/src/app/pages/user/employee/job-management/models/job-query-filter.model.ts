import { GUID } from "../../../../../shared/types/guid.type";

export interface JobQueryFilter {
  searchTerm?: string;

  sectorId?: GUID;
  managementId?: GUID;
  departmentId?: GUID;
  statusId?: GUID;
  jobCategoryId?: GUID;
  workTypeId?: GUID;
  workLocationId?: GUID;
  genderId?: GUID;
  majorId?: GUID;
  subMajorId?: GUID;

  minAge?: number;
  maxAge?: number;
  minExperienceYears?: number;
  minVacancies?: number;
  maxVacancies?: number;

  closeDateFrom?: Date;
  closeDateTo?: Date;
  publishFrom?: Date;
  publishTo?: Date;
}
