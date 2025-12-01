export enum ReviewStatus {
  Draft = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  ChangesRequested = 4,
}

export enum ReviewTargetType {
  Section = 1,
  Field = 2,
  Row = 3,
  Attachment = 4,
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

export interface ProfileApprovalDetail {
  userProfileId: string;
  userId: string;
  fullName: string;
  candidateType?: string;
  targetEntity?: string;
  submissionVersion?: number;
  submittedAtUtc?: string;
  sections: ProfileApprovalSection[];
}

export interface ProfileApprovalListItem {
  userProfileId: string;
  userId: string;
  fullName: string;
  candidateType?: string;
  targetEntity?: string;
  submittedAtUtc: string;
  pendingCount: number;
  overallStatus: ReviewStatus;
  lastUpdatedAtUtc?: string;
}
