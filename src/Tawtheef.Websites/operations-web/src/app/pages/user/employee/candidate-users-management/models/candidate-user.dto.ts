import { ProfileStatusNumber } from '../../../../../core/enums/lookups.enum';

export interface CandidateUserDto {
  id: string;
  fullNameEn: string;
  fullNameAr: string;
  email: string;
  mobileNumber: string;
  qid?: string | null;
  isBlocked: boolean;
  profileStatus?: ProfileStatusNumber | null;
  candidateProfileId?: string | null;
  userProfileId?: string | null;
}


