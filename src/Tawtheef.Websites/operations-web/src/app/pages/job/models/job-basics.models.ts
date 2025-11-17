import {GenderEnum} from '../enums/gender.enum';


export interface JobBasics {
  requestingDept: string;
  title: string;
  jobCategory: string;
  gender: GenderEnum;
  entity: string;
  major: string;
  degree: string[];
  typeOfWork: string;
  vacancies: number;
  deadline: Date | null;
}
