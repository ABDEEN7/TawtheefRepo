import { GUID } from '../../../../../shared/types/guid.type';

export interface JobCandidateListItem {
  invitationId: GUID;
  candidateId : GUID;
  candidateName: string;
  department: string;
  jobCategory: string;
  candidateCategory: string;
  candidateMajor: string;
  candidateGender: string;
  points: number;
}
