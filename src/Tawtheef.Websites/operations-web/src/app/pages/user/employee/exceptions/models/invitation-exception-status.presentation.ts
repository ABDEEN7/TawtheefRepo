import { InvitationExceptionStatus } from './invitation-exception.models';

export interface InvitationExceptionStatusPresentation {
  labelKey: string;
  className: string;
}

export const invitationExceptionStatusPresentations: Readonly<
  Record<InvitationExceptionStatus, InvitationExceptionStatusPresentation>
> = {
  [InvitationExceptionStatus.ReadyToSend]: {
    labelKey: 'EXCEPTIONS.STATUS.READY_TO_SEND',
    className: 'warning'
  },
  [InvitationExceptionStatus.InvitationSent]: {
    labelKey: 'EXCEPTIONS.STATUS.INVITATION_SENT',
    className: 'info'
  },
  [InvitationExceptionStatus.Applied]: {
    labelKey: 'EXCEPTIONS.STATUS.APPLIED',
    className: 'success'
  },
  [InvitationExceptionStatus.Expired]: {
    labelKey: 'EXCEPTIONS.STATUS.EXPIRED',
    className: 'secondary'
  },
  [InvitationExceptionStatus.Cancelled]: {
    labelKey: 'EXCEPTIONS.STATUS.CANCELLED',
    className: 'danger'
  }
};
