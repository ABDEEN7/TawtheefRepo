import {GUID} from '../../../../../shared/types/guid.type';
import { ProfileReviewTargetType } from '../../wizard-profile/models/profile-correction.model';

export { ProfileReviewTargetType as ReviewTargetTypeEnum } from '../../wizard-profile/models/profile-correction.model';

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
  NeedsCorrection = 4,
  Solved = 5
}

export enum ProfileChangeActionEnum {
  UpdateField = 1,
  ReplaceAttachment = 2,
  AddListItem = 3
}

export enum ProfileChangeRequestStatusEnum {
  Pending = 1,
  UnderReview = 2,
  Approved = 3,
  Rejected = 4,
  Canceled = 5
}

export type UserProfileStatusCode = number;  // API returns enum as number
export type ProfileSectionCode = number;     // API returns enum as number
export type ReviewTargetTypeCode = ProfileReviewTargetType;
export type ReviewStatusCode = number;       // API returns enum as number

export interface MyProfileReviewSummaryDto {
  userProfileId: GUID;
  profileStatus: UserProfileStatusCode; // numeric
  totalNotes: number;
  sections: MyProfileReviewSectionDto[];
  hasSavedChanges?: boolean;
  changedSections?: ProfileSectionCode[];
  changedItems?: MyProfileReviewChangedItemDto[];
  lastReviewerActionAtUtc?: string | null;
  lastUserChangeAtUtc?: string | null;
  canResubmit?: boolean;
}

export interface MyProfileReviewChangedItemDto {
  reviewItemId: GUID;
  section: ProfileSectionCode;
  targetType: ReviewTargetTypeCode;
  title: string;
  note?: string | null;
  fieldPath?: string | null;
  entityId?: GUID | null;
  entityName?: string | null;
  resourceId?: GUID | null;
}

export interface MyProfileReviewSectionDto {
  section: ProfileSectionCode; // numeric
  notesCount: number;
  notes: MyProfileReviewNoteDto[];
  hasUserChanges?: boolean;
  pendingItemsCount?: number;
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

export interface ProfileChangeRequestDto {
  id: GUID;
  section: ProfileSectionCode;
  action: ProfileChangeActionEnum;
  status: ProfileChangeRequestStatusEnum;
  targetKey: string;
  fieldPath?: string | null;
  entityName?: string | null;
  oldValue?: string | null;
  newValue?: string | null;
  requestedAtUtc: string;
  reviewedAtUtc?: string | null;
  reviewerNote?: string | null;
}
