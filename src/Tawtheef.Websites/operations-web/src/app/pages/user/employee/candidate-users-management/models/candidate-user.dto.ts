export interface CandidateUserDto {
  id: string;
  fullNameEn: string;
  fullNameAr: string;
  email: string;
  mobileNumber: string;
  qid?: string | null;
  isBlocked: boolean;
}
