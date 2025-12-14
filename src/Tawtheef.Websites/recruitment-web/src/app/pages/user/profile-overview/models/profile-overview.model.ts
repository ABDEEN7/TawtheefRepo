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
