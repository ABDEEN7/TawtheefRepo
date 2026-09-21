// Mirrors GeneratedSlotDto.
export interface GeneratedSlotModel {
  date: string;
  startAt: string;
  endAt: string;
  roomId?: string | null;
  roomNameAr?: string | null;
  roomNameEn?: string | null;
  remoteMeetingUrl?: string | null;
  remoteMeetingInstructions?: string | null;
}

// Mirrors ScheduleSlotPreviewDto.
export interface SlotPreviewModel {
  slot: GeneratedSlotModel;
  invitationId?: string | null;
  candidateFullNameAr?: string | null;
  candidateFullNameEn?: string | null;
  candidateQid?: string | null;
}

// Mirrors SchedulePlanPreviewDto - the single source of truth for Step 2's capacity card and
// Step 3's distribution table. Never recomputed client-side.
// eligibleMale/FemaleCount split eligibleCandidateCount by profile gender; a candidate whose profile has no
// gender counts toward the total only.
export interface SchedulePlanPreviewModel {
  totalCapacity: number;
  eligibleCandidateCount: number;
  eligibleMaleCount: number;
  eligibleFemaleCount: number;
  unassignedCandidateCount: number;
  slots: SlotPreviewModel[];
}
