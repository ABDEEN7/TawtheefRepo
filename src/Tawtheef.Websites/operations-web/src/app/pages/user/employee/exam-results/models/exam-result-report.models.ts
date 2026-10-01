import { PaginatedRequest } from '../../../../../core/models/paginated-request.model';

export interface ExamResultReportStatusOption {
  id: string;
  backendName: string;
  name: string;
  description: string | null;
  additionalData: unknown | null;
}

export interface ExamResultReportFilters extends PaginatedRequest {
  language?: string;
  search?: string;
  jobId?: string;
  statusId?: string;
  createdFrom?: string;
  createdTo?: string;
}

export interface ExamResultReportListItem {
  id: string;
  examId: string;
  examNo: string;
  examTitle: string;
  jobId: string;
  jobTitle: string;
  status: ExamResultReportStatusOption;
  candidateCount: number;
  qualifiedForInterviewCount: number;
  notQualifiedForInterviewCount: number;
  noShowCount: number;
  notCompletedCount: number;
  requiresActionCount: number;
  approvedAt: string | null;
  createdDate: string;
  lastUpdated: string;
}

export interface ExamResultLookup {
  id: string;
  backendName: string;
  nameAr: string;
  nameEn: string;
}

export interface ExamResultInterruption {
  id: string;
  interruptedAt: string;
  reason: string | null;
  status: ExamResultLookup;
  resolutionAction: ExamResultLookup | null;
  resolutionNotes: string | null;
  resolvedById: string | null;
  resolvedAt: string | null;
}

export interface ExamResultCandidate {
  id: string;
  invitationId: string;
  testSessionCandidateId: string | null;
  testAttemptId: string | null;
  candidateNameAr: string;
  candidateNameEn: string | null;
  nationalNumber: string | null;
  sessionNo: string | null;
  slotTitleAr: string | null;
  slotTitleEn: string | null;
  slotDate: string | null;
  slotStartTime: string | null;
  slotEndTime: string | null;
  attendanceStatus: ExamResultLookup | null;
  identityVerificationStatus: ExamResultLookup | null;
  candidateExecutionStatus: ExamResultLookup | null;
  attemptStatus: ExamResultLookup | null;
  attemptStartedAt: string | null;
  attemptCompletedAt: string | null;
  specializedScore: number | null;
  isSpecializedQualified: boolean | null;
  educationalScore: number | null;
  skillsScore: number | null;
  finalScore: number | null;
  rank: number | null;
  resultStatus: ExamResultLookup;
  hasOpenInterruption: boolean;
  interruptions: ExamResultInterruption[];
  snapshotAt: string;
}

export interface ExamResultReportDetails {
  id: string;
  examId: string;
  examNo: string;
  examTitleAr: string;
  examTitleEn: string | null;
  jobId: string;
  jobNameAr: string;
  jobNameEn: string | null;
  specializedQualificationThreshold: number;
  status: ExamResultLookup;
  approvedById: string | null;
  approvedAt: string | null;
  closedAt: string | null;
  decisionNotes: string | null;
  createdDate: string;
  candidates: ExamResultCandidate[];
}

export interface ExamResultAttemptOption {
  questionRevisionOptionId: string;
  optionTextAr: string | null;
  optionTextEn: string | null;
  displayOrder: number;
  isSelected: boolean;
}

export interface ExamResultAttemptQuestion {
  testAttemptQuestionId: string;
  orderNo: number;
  questionRevisionId: string;
  questionTextAr: string | null;
  questionTextEn: string | null;
  questionType: ExamResultLookup;
  category: ExamResultLookup | null;
  selectedQuestionRevisionOptionId: string | null;
  savedAt: string | null;
  isAnswered: boolean;
  isCorrect: boolean | null;
  options: ExamResultAttemptOption[];
}

export interface ExamResultAttemptPart {
  testAttemptPartId: string;
  partNo: number;
  status: ExamResultLookup;
  startedAt: string | null;
  completedAt: string | null;
  durationMinutesSnapshot: number | null;
  effectiveDeadlineAt: string | null;
  questions: ExamResultAttemptQuestion[];
}

export interface ExamResultCandidateAttemptDetails {
  examResultCandidateId: string;
  invitationId: string;
  testSessionCandidateId: string | null;
  testAttemptId: string | null;
  resultStatus: ExamResultLookup;
  specializedScore: number | null;
  educationalScore: number | null;
  skillsScore: number | null;
  finalScore: number | null;
  rank: number | null;
  snapshotAt: string;
  hasAttempt: boolean;
  attemptStatus: ExamResultLookup | null;
  startedAt: string | null;
  completedAt: string | null;
  parts: ExamResultAttemptPart[];
}

export interface ExamResultRescheduleOption {
  testSessionId: string;
  sessionNo: string;
  testSlotId: string;
  slotTitleAr: string;
  slotTitleEn: string | null;
  slotDate: string;
  startTime: string;
  endTime: string;
  roomId: string;
  roomNameAr: string;
  roomNameEn: string | null;
  capacity: number;
  assignedCount: number;
  availableCapacity: number;
}
