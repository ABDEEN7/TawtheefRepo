import {GUID} from '../../../../../../shared/types/guid.type';
import {dropdownOptionsModel} from '../../../../../../shared/models/dropdown-options.model';

export enum ReviewStatus {
  NotReviewed = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  NeedsCorrection = 4,
  ChangesRequested = 5,
}

export enum ReviewTargetType {
  Section = 1,
  Field = 2,
  Row = 3,
  Attachment = 4,
}

export interface FileRefDto {
  resourceId: string;
  fileName: string;
  url: string;
}

export interface ProfileApprovalItem {
  reviewItemId: string;
  targetType: ReviewTargetType;
  status: ReviewStatus;
  title: string;
  note?: string;
  resourceId?: string;
  resourceUrl?: string;
  entityId?: string;
  entityName?: string;
  oldValue?: string;
  newValue?: string;
  version: number;
  approvedAtVersion?: number;
  reviewedAtUtc?: string;
}

export interface ProfileApprovalSection {
  section: number;
  sectionReview?: ProfileApprovalItem | null;
  items: ProfileApprovalItem[];
  hasAttachments: boolean;
}

export interface BasicInformationSnapshot {
  candidateType?: string;
  targetEntity?: string;
  office?: string;
  resumeAttachment?: FileRefDto | null;
  nationalCard?: FileRefDto | null;
  birthdayCertificate?: FileRefDto | null;
  marriageCertificate?: FileRefDto | null;

  fullNameAr?: string;
  fullNameEn?: string;
  nationalNumber?: string;
  qidExpiry?: string;
  birthDate?: string;
  nationality?: string;
  gender?: string;
  religion?: string;
  maritalStatus?: string;
  childrenCount?: number;
  hasDisability?: boolean;
  disabilityDetails?: string;

  sponsorType?: string;
  sponsorEmployerName?: string;
  sponsorEmployerNumber?: string;
  sponsorQidExpiry?: string;
  sponsorCard?: FileRefDto | null;

  residenceCountry?: string;

  phoneNumber?: string;
  email?: string;
  address?: string;
  residenceAddress?: ResidenceAddressDto | null;
  interviewLocation?: string;
}

export interface ProfileApprovalData {
  basicInformation: BasicInformationSnapshot;
  qualifications: any[];
  experiences: any[];
  trainingCourses: any[];
  professionalCertificatesAndAwards: any[];
  skills: any[];
  languages: any[];
  attachments: any[];
  profilePhoto?: string | null;
}
export interface ProfileApprovalDetail {
  userProfileId: string;
  userId: string;
  fullName: string;
  candidateType?: string;
  targetEntity?: string;
  targetEntityCategory?: string;
  specialization?: string;

  submissionVersion?: number;
  submittedAtUtc?: string;

  profile: ProfileApprovalDataDto;
  approvedProfile?: ProfileApprovalDataDto | null;

  sections: ProfileApprovalSection[];

  isInitialReview: boolean;
  isPartialReview: boolean;
}
export interface ProfileApprovalDataDto {
  basicInformation: BasicInformationSnapshot;
  qualifications: QualificationDto[];
  experiences: ExperienceDto[];
  trainingCourses: TrainingCourseDto[];
  professionalCertificatesAndAwards: AchievementDto[];
  skills: SkillDto[];
  languages: LanguageDto[];
  attachments: AdditionalAttachmentDto[];
  profilePhoto?: string;
}
export interface BasicInformationSnapshot {
  fullNameAr?: string;
  fullNameEn?: string;
  nationalNumber?: string;

  /** ISO date string: yyyy-MM-dd */
  birthDate?: string;

  nationality?: string;
  gender?: string;
  religion?: string;
  maritalStatus?: string;
  childrenCount?: number;

  candidateType?: string;
  targetEntity?: string;

  resumeAttachment?: FileRefDto | null;
  nationalCard?: FileRefDto | null;
  residenceAddress?: ResidenceAddressDto | null;
  birthdayCertificate?: FileRefDto | null;
  marriageCertificate?: FileRefDto | null;
}
export interface ResidenceAddressDto {
  naZone: number,
  naStreet: number,
  naBuilding: number,
  naUnit: number,
  certificateId?: string | null;
  certificate?: FileRefDto | null;
}
export interface ProfileApprovalListItem {
  userProfileId: string;
  userId: string;
  fullName: string;
  candidateType?: string;
  targetEntity?: string;
  specialization?: string;
  submittedAtUtc: string;
  profileStatus?: number;
  pendingCount: number;
  overallStatus: ReviewStatus;
  lastUpdatedAtUtc?: string;
  allowedOperations?: string[];
}

export interface ProfileApprovalListFilter {
  search?: string;
  specialization?: string;
  status?: ReviewStatus | '';
  targetEntity?: string;
  candidateType?: string;
  sort?: 'name' | 'status' | 'entity' | 'date';
  sortDirection?: 'asc' | 'desc';
}

export type FinalApprovalAction =
  | 'ApproveProfile'
  | 'NeedsCorrection'
  | 'RejectProfile'
  | 'BlockProfile'
  | 'ExceptionalApproval';

export interface AdditionalAttachmentDto {
  id: GUID;
  title?: string | null;
  file: FileRefDto;
}
export interface LanguageDto {
  id: GUID;
  languageId: string;
  speakingLevelId: string;
  speakingLevel: dropdownOptionsModel;
  writingLevelId: string;
  writingLevel: dropdownOptionsModel;
  readingLevelId: string;
  readingLevel: dropdownOptionsModel;
  isNative: boolean;
}
export interface SkillDto {
  id: GUID;
  skillId: string;
  skill: dropdownOptionsModel;
  levelId: string;
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
  qualificationId?: string | null;
  qualification?: QualificationDto | null;
  attachment?: FileRefDto | null;
}
export interface QualificationDto {
  id: GUID;
  degreeId?: string | null;
  degree?: dropdownOptionsModel | null;
  gradCountryId?: string | null;
  gradCountry?: dropdownOptionsModel | null;
  universityId?: string | null;
  university?: dropdownOptionsModel | null;
  majorId?: string | null;
  major?: dropdownOptionsModel | null;
  subMajorId?: string | null;
  subMajor?: dropdownOptionsModel | null;
  graduationYear?: number | null;
  studyTypeId?: string | null;
  studyType?: dropdownOptionsModel | null;
  gpa?: number | null;
  gradeId?: string | null;
  grade?: dropdownOptionsModel | null;
  attachment?: FileRefDto | null;
}
export interface AchievementDto{
  id: GUID;
  achievementTypeId: string;
  achievementType?: dropdownOptionsModel | null;
  title: string;
  issuingAuthority: string;
  countryId?: string | null;
  country?: dropdownOptionsModel | null;
  issueDate: string;
  description: string;
  attachment?: FileRefDto | null;
}
