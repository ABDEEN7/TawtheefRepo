import { InviteStatus } from "../types/invite-status.type";

export interface Invite {
  id: number;
  jobId: number;
  profileId: number;
  status: InviteStatus;
  sentAt: string;
  viewedAt?: string;
  respondedAt?: string;
}
