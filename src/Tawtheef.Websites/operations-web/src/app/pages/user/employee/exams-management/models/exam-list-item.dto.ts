import { dropdownOptionsModel } from '../../../../../shared/models/dropdown-options.model';

export interface ExamListItemDto {
  id: string;
  examNo: string;
  jobTitle: string;
  specializations: string[];
  totalQuestions: number;
  totalDurationMinutes: number;
  status: dropdownOptionsModel;
  createdDate: string;
  lastUpdated: string;
}

export interface ExamFilters {
  pageNumber: number;
  pageSize: number;
  language: string;
  search?: string;
  specializationId?: string;
  statusId?: string;
  createdFrom?: string;
  createdTo?: string;
}
