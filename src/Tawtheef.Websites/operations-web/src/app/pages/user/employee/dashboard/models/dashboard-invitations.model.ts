import { InvitationSource } from '../../../../../core/enums/invitation-source.enum';

export interface InvitationKpis {
  totalInvitations: number;
  acceptedInvitations: number;
  pendingInvitations: number;
  pendingAttachmentApproval: number;
  expiredInvitations: number;
  rejectedInvitations: number;
}

export interface LatestInvitation {
  invitationId: string;
  source: InvitationSource;
  title: string;
  status: string;
  sentDate: string;
  actionKey?: string;
}
