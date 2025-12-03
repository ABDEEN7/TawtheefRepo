import { GUID } from "../../../shared/types/guid.type";

export interface JobBasics {
  title: string;                    
  vacancies: number;               
  deadline: Date | null;            
  sectorId: GUID;                   
  managementId: GUID;               
  requestingDepartmentId: GUID;     
  jobCategoryId: GUID;              
  workTypeId: GUID;                
  majorId: GUID;                    
  subMajorId?: GUID;                
  genderId?: GUID;                  
  degreeIds: GUID[];               
  workLocationId: GUID;                
  minimumExperienceYears?: number;   
  minimumAge?: number;               
  maximumAge?: number;               
}