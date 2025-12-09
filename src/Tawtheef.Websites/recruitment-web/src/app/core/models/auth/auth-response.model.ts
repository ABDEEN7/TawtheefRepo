import {UserInfoModel} from "../../../shared/models/user-info.model";
import {TokenModel} from "./token.model";
import {dropdownOptionsModel} from '../../../shared/models/dropdown-options.model';
import {GUID} from '../../../shared/types/guid.type';

export interface AuthResponse {
  //requiresProfileCompletion: boolean;
  user: UserInfoModel;
  token: TokenModel;
  //missingFields: string[];
  //prefill: PrefillData;
}

export interface AuthBootstrap {
  requiresProfileCompletion: boolean;
  missingFields: string[];
  prefill:  PrefillData | null;
}

export interface PrefillData
{
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
}

export interface ProfileStatusDto {
  isComplete: boolean;
  missing: string[];
  isDraft: boolean;
  avatar?: string | null;
  fullNameAr?: string | null;
  fullNameEn?: string | null;
  email?: string | null;
  emailVerified: boolean;
  phone?: string | null;
  phoneVerified: boolean;
  // ===== Scalars =====
  candidateTypeId?: string | null;
  targetEntityId?: string | null;
  officeId?: string | null;

  nationalNumber?: string | null;
  qidExpiry?: string | null;
  birthDate?: string | null;
  nationalityId?: string | null;
  genderId?: string | null;
  religionId?: string | null;
  maritalStatusId?: string | null;

  childrenCount: number;

  residenceCountryId?: string | null;
  interviewLocationId?: string | null;

  address?: string | null;
  naZone?: string | null;
  naStreet?: string | null;
  naBuilding?: string | null;
  naUnit?: string | null;

  hasDisability: boolean;
  disabilityDetails?: string | null;

  sponsorTypeId?: string | null;
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
}
export interface AdditionalAttachmentDto {
  id: GUID;
  title?: string | null;
  file: FileRefDto;
}
export interface LanguageDto {
  id: GUID;
  languageId: string;
  speakingLevelId: string;
  writingLevelId: string;
  readingLevelId: string;
  isNative: boolean;
}
export interface SkillDto {
  id: GUID;
  skillId: string;
  skill: dropdownOptionsModel;
  levelId: string;
}
export interface TrainingCourseDto {
  id: GUID;
  title?: string | null;
  provider?: string | null;
  countryId: GUID;
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
  startDate?: string | null;
  endDate?: string | null;
  description?: string | null;
  isCurrent: boolean;
  qualificationId?: string | null;
  attachment?: FileRefDto | null;
}
export interface QualificationDto {
  id: GUID;
  degreeId?: string | null;
  gradCountryId?: string | null;
  universityId?: string | null;
  university?: dropdownOptionsModel | null;
  majorId?: string | null;
  major?: dropdownOptionsModel | null;
  subMajorId?: string | null;
  subMajor?: dropdownOptionsModel | null;
  graduationYear?: number | null;
  studyTypeId?: string | null;
  gpa?: number | null;
  gradeId?: string | null;
  attachment?: FileRefDto | null;
}
export interface FileRefDto {
  resourceId: GUID;
  fileName: string;
  url?: string | null;
}
export interface AchievementDto{
  id: GUID;
  achievementTypeId: string;
  title: string;
  issuingAuthority: string;
  countryId?: string | null;
  issueDate: string;
  description: string;
  attachment?: FileRefDto | null;
}
