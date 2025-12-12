export interface ProfilePendingItem {
  reviewItemId: string;
  targetType: number;
  status: number;
  title: string;
  oldValue?: string | null;
  newValue?: string | null;
}

export interface ProfileOverviewSection {
  section: number;
  pendingItems: ProfilePendingItem[];
}

export interface ProfileOverview {
  userProfileId: string;
  status: number;
  hasPendingChanges: boolean;
  sections: ProfileOverviewSection[];
}
