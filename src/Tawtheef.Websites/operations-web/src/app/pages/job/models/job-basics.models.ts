import { GUID } from "../../../shared/types/guid.type";

export interface JobBasics {
  requestingDepartmentId: GUID;
  title: string;
  jobCategoryId: GUID;
  genderId: GUID;
  workLocationId: GUID;
  majorId: GUID;
  workTypeId: GUID;
  vacancies: number;
  deadline: Date | null;
  degreeIds: GUID[];
}
