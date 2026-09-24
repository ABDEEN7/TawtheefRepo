import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface TestSessionListItemDto {
  id: string;
  sessionNo: number;
  examId: string;
  examName: string;
  jobId: string;
  jobTitle: string;
  sessionDate: string;
  period: string;
  roomId: string;
  roomName: string;
  candidateCount: number;
  roomHeadName?: string;
  status: dropdownOptionsModel;
}

export interface TestSessionFilters {
  pageNumber: number;
  pageSize: number;
  language: string;
  searchText?: string;
  examId?: string;
  jobId?: string;
  roomId?: string;
  fromDate?: string;
  toDate?: string;
  periodId?: string;
  statusId?: string;
  nationalityId?: string;
  genderId?: string;
}

export interface TestSessionLookupsDto {
  exams: dropdownOptionsModel[];
  jobs: dropdownOptionsModel[];
  rooms: dropdownOptionsModel[];
  statuses: dropdownOptionsModel[];
  periods: dropdownOptionsModel[];
  nationalities: dropdownOptionsModel[];
  genders: dropdownOptionsModel[];
}
