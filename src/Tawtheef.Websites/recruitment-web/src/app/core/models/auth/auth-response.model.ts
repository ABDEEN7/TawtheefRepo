import {UserInfoModel} from "../../../shared/models/user-info.model";
import {TokenModel} from "./token.model";
import {dropdownOptionsModel} from '../../../shared/models/dropdown-options.model';
import {GUID} from '../../../shared/types/guid.type';

export interface AuthResponse {
  user: UserInfoModel;
  token: TokenModel;
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
  isKawaderQid?: boolean | null;
}

export interface ProfileStatusDto {
  isComplete: boolean;
  agreedToTerms: boolean;
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
  candidateType?: dropdownOptionsModel | null;
  targetEntityId?: GUID | null;
  targetEntity?: dropdownOptionsModel | null;
  officeId?: GUID | null;
  office?: dropdownOptionsModel | null;

  nationalNumber?: string | null;
  qidExpiry?: string | null;
  birthDate?: string | null;
  nationalityId?: GUID | null;
  nationality?: dropdownOptionsModel | null;
  genderId?: GUID | null;
  gender?: dropdownOptionsModel | null;
  religionId?: GUID | null;
  religion?: dropdownOptionsModel | null;
  maritalStatusId?: GUID | null;
  maritalStatus?: dropdownOptionsModel | null;

  residenceCountryId?: GUID | null;
  residenceCountry?: dropdownOptionsModel | null;
  interviewLocationId?: GUID | null;
  interviewLocation?: dropdownOptionsModel | null;

  address?: string | null;
  naZone?: string | null;
  naStreet?: string | null;
  naBuilding?: string | null;
  naUnit?: string | null;

  hasDisability: boolean;
  disabilityDetails?: string | null;

  sponsorTypeId?: GUID | null;
  sponsorType?: dropdownOptionsModel | null;
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
  language: dropdownOptionsModel;
  speakingLevelId: GUID;
  speakingLevel: dropdownOptionsModel;
  writingLevelId: GUID;
  writingLevel: dropdownOptionsModel;
  readingLevelId: GUID;
  readingLevel: dropdownOptionsModel;
}
export interface SkillDto {
  id: GUID;
  skillId: GUID;
  skill: dropdownOptionsModel;
  levelId: GUID;
  level: dropdownOptionsModel;
}
export interface TrainingCourseDto {
  id: GUID;
  title?: string | null;
  provider?: string | null;
  countryId: GUID;
  country?: dropdownOptionsModel | null;
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
  country?: dropdownOptionsModel | null;
  startDate?: string | null;
  endDate?: string | null;
  description?: string | null;
  isCurrent: boolean;
  qualificationId?: GUID | null;
  qualification?: dropdownOptionsModel | null;
  attachment?: FileRefDto | null;
}
export interface QualificationDto {
  id: GUID;
  degreeId?: GUID | null;
  degree?: dropdownOptionsModel | null;
  gradCountryId?: GUID | null;
  gradCountry?: dropdownOptionsModel | null;
  universityId?: GUID | null;
  university?: dropdownOptionsModel | null;
  majorId?: GUID | null;
  major?: dropdownOptionsModel | null;
  subMajorId?: GUID | null;
  subMajor?: dropdownOptionsModel | null;
  graduationYear?: number | null;
  studyTypeId?: GUID | null;
  studyType?: dropdownOptionsModel | null;
  gpa?: number | null;
  gradeId?: GUID | null;
  grade?: dropdownOptionsModel | null;
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
  country?: dropdownOptionsModel | null;
  issueDate: string;
  description: string;
  attachment?: FileRefDto | null;
}
