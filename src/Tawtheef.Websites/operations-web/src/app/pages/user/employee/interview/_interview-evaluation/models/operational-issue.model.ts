import { OperationalIssueStatus, OperationalIssueType } from './enums';

// Mirrors OperationalIssueDto.
export interface OperationalIssueModel {
  id: string;
  interviewAppointmentId: string;
  issueType: OperationalIssueType;
  description: string | null;
  isBlocking: boolean;
  status: OperationalIssueStatus;
  resolvedById: string | null;
  resolvedAt: string | null;
  resolutionNotes: string | null;
  createdDate: string;
}
