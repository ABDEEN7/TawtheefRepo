

export interface JobBasics {
  requestingDept: string;
  title: string;
  jobCategory: string;
  gender: string[];
  entity: string;
  major: string;
  degree: string[];
  typeOfWork: string;
  vacancies: number;
  deadline: Date | null;
}
