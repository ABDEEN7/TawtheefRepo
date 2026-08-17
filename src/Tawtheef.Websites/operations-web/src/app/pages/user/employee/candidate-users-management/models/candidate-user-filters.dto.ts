import { ProfileStatus } from '../../../../../core/enums/lookups.enum';

export enum CandidateUsersResultScope {
  Default = 'Default',
  AccessibleProfiles = 'AccessibleProfiles'
}

export interface CandidateUserFilters {
  pageNumber: number;
  pageSize: number;
  name?: string;
  email?: string;
  qid?: string;
  mobileNumber?: string;
  profileStatus?: ProfileStatus;
  year?: number;
  scope: CandidateUsersResultScope;
}
