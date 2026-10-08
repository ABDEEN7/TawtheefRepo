export type CandidateExamExecutionPreparationState =
  | 'preparing'
  | 'loadingQuestion'
  | 'ready'
  | 'connectionError'
  | 'questionError';

export type CandidateExamAnswerSaveState = 'idle' | 'saving' | 'saved' | 'error';
