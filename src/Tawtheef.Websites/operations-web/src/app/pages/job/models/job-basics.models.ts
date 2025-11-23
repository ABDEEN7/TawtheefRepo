import { GUID } from "../../../shared/types/guid.type";

export interface JobBasics {
  requestingDeptId: GUID;
  title: string;
  jobCategoryId: GUID;
  genderId: GUID;
  entityId: GUID;
  majorId: GUID;
  workTypeId: GUID;
  vacancies: number;
  deadline: Date | null;
}
