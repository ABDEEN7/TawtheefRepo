import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';

export interface CandidateInvitationModel {
  invitationId: string; // Guid
  jobTitle: string;
  departmentName: string;
  jobCategory: string;
  jobCategoryBackendName: string;
  invitationStatus: dropdownOptionsModel;
  createDate: string;           // ISO date string
}
