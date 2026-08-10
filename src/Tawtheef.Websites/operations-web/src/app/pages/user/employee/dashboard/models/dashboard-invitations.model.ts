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
  title: string;
  status: string;
  sentDate: string;
  actionKey?: string;
}
