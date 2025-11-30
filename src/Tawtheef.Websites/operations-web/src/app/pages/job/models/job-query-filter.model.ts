export interface JobQueryFilter {
  searchTerm?: string;
  departmentId?: string;   
  statusId?: string;
  jobCategoryId?: string;
  workTypeId?: string;
  deadlineFrom?: string; 
  deadlineTo?: string;
  minVacancies?: number;
  maxVacancies?: number;
}
