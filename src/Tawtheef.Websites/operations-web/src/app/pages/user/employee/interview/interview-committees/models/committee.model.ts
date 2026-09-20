import { CommitteeStatus } from './enums';

export interface CommitteeModel {
  id: string;
  code: string;
  jobId: string;
  jobTitleNameAr?: string | null;
  jobTitleNameEn?: string | null;
  interviewTemplateId: string;
  interviewTemplateTitleAr: string;
  interviewTemplateTitleEn?: string | null;
  committeeTypeId: string;
  committeeTypeNameAr: string;
  committeeTypeNameEn?: string | null;
  nameAr: string;
  nameEn?: string | null;
  chairNameAr?: string | null;
  chairNameEn?: string | null;
  memberCount: number;
  scopeDescription?: string | null;
  notes?: string | null;
  status: CommitteeStatus;
  isActive: boolean;
  approvedById?: string | null;
  approvedAt?: string | null;
  closedAt?: string | null;
  decisionNotes?: string | null;
}
