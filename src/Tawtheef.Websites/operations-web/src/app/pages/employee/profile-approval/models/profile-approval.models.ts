export enum ReviewStatus {
  NotReviewed = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  NeedsCorrection = 4,
  ChangesRequested = 4,
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
  email?: string;
  phoneNumber?: string;
  residenceCountry?: string;
  address?: string;
  interviewLocation?: string;
  hasDisability?: boolean;
  disabilityDetails?: string;
  sponsorType?: string;
  sponsorEmployerName?: string;
  sponsorEmployerNumber?: string;
  sponsorQidExpiry?: string;
  sponsorCard?: FileRefDto | null;
  candidateType?: string;
  targetEntity?: string;
  targetEntityCategory?: string;
  cvSummary?: string;
  resumeAttachment?: FileRefDto | null;
  nationalCard?: FileRefDto | null;
  residenceAddressCertificate?: FileRefDto | null;
  birthdayCertificate?: FileRefDto | null;
  marriageCertificate?: FileRefDto | null;
}

export interface ProfileApprovalData {
  basicInformation: BasicInformationSnapshot;
  qualifications: any[];
  experiences: any[];
  trainingCourses: any[];
  professionalCertificatesAndAwards: any[];
  skillsAndLanguages: any[];
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

  profile: any;
  approvedProfile?: any | null;

  sections: ProfileApprovalSection[];

  isInitialReview: boolean;
  isPartialReview: boolean;
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

export interface FinalApprovalRequest {
  action: FinalApprovalAction;
  notes?: string;
  summary?: string;
  rejectionDocument?: File | null;
  exceptionalFile?: File | null;
  needsCorrectionItems?: string[];
}
