import {GUID} from '../../../../../shared/types/guid.type';

export enum UserProfileStatusEnum {
  InCreation = 0,
  Submitted = 1,
  UnderReview = 2,
  RequiresUpdate = 3,
  Approved = 4,
  Rejected = 5,
  AdminCancelled = 6
}

export enum ProfileSectionEnum {
  Prerequisites = 1,
  Personal = 2,
  Contact = 3,
  Qualifications = 4,
  Experience = 5,
  TrainingCourses = 6,
  CertificatesAndAwards = 7,
  Skills = 8,
  Languages = 9,
  Attachments = 10
}

export enum ReviewStatusEnum {
  NotReviewed = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  NeedsCorrection = 4
}

export enum ReviewTargetTypeEnum {
  Section = 1,
  Field = 2,
  Row = 3,
  Attachment = 4
}

export type UserProfileStatusCode = number;  // API returns enum as number
export type ProfileSectionCode = number;     // API returns enum as number
export type ReviewTargetTypeCode = number;   // API returns enum as number
export type ReviewStatusCode = number;       // API returns enum as number

export interface MyProfileReviewSummaryDto {
  userProfileId: GUID;
  profileStatus: UserProfileStatusCode; // numeric
  totalNotes: number;
  sections: MyProfileReviewSectionDto[];
}

export interface MyProfileReviewSectionDto {
  section: ProfileSectionCode; // numeric
  notesCount: number;
  notes: MyProfileReviewNoteDto[];
}

export interface MyProfileReviewNoteDto {
  reviewItemId: GUID;
  targetType: ReviewTargetTypeCode; // numeric
  status: ReviewStatusCode; // numeric
  title: string;
  note?: string | null;

  fieldPath?: string | null;
  entityId?: GUID | null;
  entityName?: string | null;
  resourceId?: GUID | null;

  reviewedAtUtc?: string | null;
}

export interface ProfilePendingItem {
  reviewItemId: string;
  targetType: number;
  status: number;
  title: string;
  note?: string | null;
  oldValue?: string | null;
  newValue?: string | null;
  reviewedAtUtc?: string | null;
}

export interface ProfileOverviewSection {
  section: number;
  pendingItems: ProfilePendingItem[];
}

export interface ProfileRequestProgress {
  latestVersion: number;
  pendingCount: number;
  needsCorrectionCount: number;
  rejectedCount: number;
  approvedCount: number;
  lastSubmittedAtUtc?: string | null;
  lastDecisionAtUtc?: string | null;
  latestReviewerNote?: string | null;
  latestStatus?: number | null;
  requiresUserAction: boolean;
}

export interface ProfileOverview {
  userProfileId: string;
  status: number;
  hasPendingChanges: boolean;
  requestProgress: ProfileRequestProgress;
  sections: ProfileOverviewSection[];
}
