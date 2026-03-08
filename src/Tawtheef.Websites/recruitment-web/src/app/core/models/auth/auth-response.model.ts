import { UserInfoModel } from "../../../shared/models/user-info.model";
import { TokenModel } from "./token.model";
import { DropdownOptionVM } from '../../../shared/models/dropdown-options.model';
import { GUID } from '../../../shared/types/guid.type';

export interface AuthResponse {
  requiresProfileCompletion: boolean;
  user: UserInfoModel;
  token: TokenModel;
}

export interface AuthBootstrap {
  requiresProfileCompletion: boolean;
  missingFields: string[];
  prefill: PrefillData | null;
}

export interface PrefillData {
  email?: string | null;
  emailVerified?: boolean | null;
  fullName?: string | null;
  avatar?: string | null;
  phone?: string | null;
  phoneVerified?: boolean | null;
  nationality?: string | null;
  qid?: string | null;
  gender?: string | null;
  dob?: string | null;
  locale?: string | null;
  provider?: string | null;
  isKawaderQid?: boolean | null;
}

export interface ProfileStatusDto {
  isComplete: boolean;
  agreedToTerms: boolean;
  status: number;
  missing: string[];
  isDraft: boolean;
  avatar?: string | null;
  availableForRecruitment?: boolean | null;
  provider: string;
  fullNameAr?: string | null;
  fullNameEn?: string | null;
  email?: string | null;
  emailVerified: boolean;
  phone?: string | null;
  phoneVerified: boolean;
  // ===== Scalars =====
  candidateTypeId?: GUID | null;
  candidateType?: DropdownOptionVM | null;
  targetEntityId?: GUID | null;
  targetEntity?: DropdownOptionVM | null;
  officeId?: GUID | null;
  office?: DropdownOptionVM | null;

  nationalNumber?: string | null;
  qidExpiry?: string | null;
  birthDate?: string | null;
  nationalityId?: GUID | null;
  nationality?: DropdownOptionVM | null;
  genderId?: GUID | null;
  gender?: DropdownOptionVM | null;
  religionId?: GUID | null;
  religion?: DropdownOptionVM | null;
  maritalStatusId?: GUID | null;
  maritalStatus?: DropdownOptionVM | null;

  residenceCountryId?: GUID | null;
  residenceCountry?: DropdownOptionVM | null;
  interviewLocationId?: GUID | null;
  interviewLocation?: DropdownOptionVM | null;

  address?: string | null;
  naZone?: string | null;
  naStreet?: string | null;
  naBuilding?: string | null;
  naUnit?: string | null;

  hasDisability: boolean;
  disabilityDetails?: string | null;

  sponsorTypeId?: GUID | null;
  sponsorType?: DropdownOptionVM | null;
  sponsorEmployerName?: string | null;
  sponsorEmployerNumber?: string | null;
  sponsorQidExpiry?: string | null;
  sponsorCard?: FileRefDto | null;

  // ===== Attachments =====
  resumeAttachment?: FileRefDto | null;
  nationalCard?: FileRefDto | null;
  residenceAddressCertificate?: FileRefDto | null;
  birthdayCertificate?: FileRefDto | null;
  marriageCertificate?: FileRefDto | null;
  additionalAttachments?: AdditionalAttachmentDto[] | null;

  // ===== Collections =====
  qualifications?: QualificationDto[] | null;
  experiences?: ExperienceDto[] | null;
  trainingCourses?: TrainingCourseDto[] | null;
  skills?: SkillDto[] | null;
  languages?: LanguageDto[] | null;
  achievements: AchievementDto[];
  isKawaderQid?: boolean | null;
}
export interface AdditionalAttachmentDto {
  id: GUID;
  title?: string | null;
  file: FileRefDto;
}
export interface LanguageDto {
  id: GUID;
  languageId: GUID;
  language: DropdownOptionVM;
  speakingLevelId: GUID;
  speakingLevel: DropdownOptionVM;
  writingLevelId: GUID;
  writingLevel: DropdownOptionVM;
  readingLevelId: GUID;
  readingLevel: DropdownOptionVM;
}
export interface SkillDto {
  id: GUID;
  skillId: GUID;
  skill: DropdownOptionVM;
  levelId: GUID;
  level: DropdownOptionVM;
}
export interface TrainingCourseDto {
  id: GUID;
  title?: string | null;
  provider?: string | null;
  countryId: GUID;
  country?: DropdownOptionVM | null;
  startDate?: string | null;
  endDate?: string | null;
  description?: string | null;
  attachment?: FileRefDto | null;
}
export interface ExperienceDto {
  id: GUID;
  employerName?: string | null;
  jobTitle?: string | null;
  countryId: GUID;
  country?: DropdownOptionVM | null;
  startDate?: string | null;
  endDate?: string | null;
  description?: string | null;
  isCurrent: boolean;
  qualificationId?: GUID | null;
  qualification?: DropdownOptionVM | null;
  attachment?: FileRefDto | null;
}
export interface QualificationDto {
  id: GUID;
  degreeId?: GUID | null;
  degree?: DropdownOptionVM | null;
  gradCountryId?: GUID | null;
  gradCountry?: DropdownOptionVM | null;
  universityId?: GUID | null;
  university?: DropdownOptionVM | null;
  majorId?: GUID | null;
  major?: DropdownOptionVM | null;
  subMajorId?: GUID | null;
  subMajor?: DropdownOptionVM | null;
  graduationYear?: number | null;
  studyTypeId?: GUID | null;
  studyType?: DropdownOptionVM | null;
  gpa?: number | null;
  gradeId?: GUID | null;
  grade?: DropdownOptionVM | null;
  attachment?: FileRefDto | null;
}
export interface FileRefDto {
  resourceId: GUID;
  fileName: string;
  url?: string | null;
}
export interface AchievementDto {
  id: GUID;
  achievementTypeId: GUID;
  achievementType?: DropdownOptionVM | null;
  title: string;
  issuingAuthority: string;
  countryId: GUID;
  country?: DropdownOptionVM | null;
  issueDate: string;
  description: string;
  relatedToSpecialization: boolean;
  attachment?: FileRefDto | null;
}
