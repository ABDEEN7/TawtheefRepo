
export type CandidateEligibilityStatusCode =
  | 'PASSED'
  | 'FAILED'
  | 'NOT_APPLICABLE';

export const candidateEligibilityStatusCodes = {
  PASSED: 'PASSED' as const,
  FAILED: 'FAILED' as const,
  NOT_APPLICABLE: 'NOT_APPLICABLE' as const
};

export interface CandidateEligibilityCondition {
  code: string;

  label: string;
  status: CandidateEligibilityStatusCode;

  expectedValue?: string;
  actualValue?: string;
  failureReason?: string;

  expectedValueParams?: Record<string, unknown>;
  actualValueParams?: Record<string, unknown>;
  failureReasonParams?: Record<string, unknown>;
}