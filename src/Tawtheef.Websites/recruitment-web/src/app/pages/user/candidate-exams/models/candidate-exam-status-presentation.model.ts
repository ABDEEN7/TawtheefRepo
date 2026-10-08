export type CandidateExamStatusTone =
  | 'success'
  | 'warning'
  | 'danger'
  | 'info'
  | 'neutral'
  | 'secondary';

export interface CandidateExamStatusPresentation {
  readonly labelKey: string;
  readonly tone: CandidateExamStatusTone;
}
