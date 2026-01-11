import {
  ProfileChangeActionEnum,
  ProfileChangeRequestStatusEnum,
  ProfileSectionCode
} from '../models/profile-overview.model';
import {GUID} from '../../../../../shared/types/guid.type';

export interface changeRequestDto {
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
  statusLabelKey: string;
  statusSeverity: "success" | "warn" | "danger" | "secondary";
  sectionLabelKey: string
}
