import { GUID } from "../../../shared/types/guid.type";
import { InviteStatus } from "../types/invite-status.type";

export interface Invite {
  id: GUID;
  jobId: GUID;
  profileId: GUID;
  status: InviteStatus;
  sentAt: string;
  viewedAt?: string;
  respondedAt?: string;
}
