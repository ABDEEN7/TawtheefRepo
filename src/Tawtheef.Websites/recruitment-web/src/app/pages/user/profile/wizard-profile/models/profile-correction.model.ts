import { GUID } from '../../../../../shared/types/guid.type';

export enum ProfileReviewTargetType {
  Section = 1,
  Field = 2,
  Row = 3,
  Attachment = 4,
}

export interface ProfileCorrectionContext {
  section: number;
  targetType: ProfileReviewTargetType;
  fieldPath?: string | null;
  entityId?: GUID | null;
  resourceId?: GUID | null;
  note?: string | null;
  status?: number;
}

export const PROFILE_REVIEW_SECTION_DATA_FIELD = 'SectionData';
