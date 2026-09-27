export interface ApproveExamResultReportRequest {
  decisionNotes: string | null;
}

export interface ReturnExamResultReportForProcessingRequest {
  reason: string | null;
}

export interface ResolveExamResultCandidateAsNotCompletedRequest {
  resolutionNotes: string | null;
}

export interface AcceptExamResultCandidateCalculatedScoreRequest {
  resolutionNotes: string | null;
}

export interface RescheduleExamResultCandidateRequest {
  targetTestSessionId: string;
  reason: string | null;
}
