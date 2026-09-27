export interface ValidateCandidateExamAccessCodeRequest {
  readonly accessCode: string;
}

export interface SaveCandidateExamAnswerRequest {
  readonly selectedOptionId: string;
}

export interface NavigateCandidateExamRequest {
  readonly targetQuestionNumber: number;
}
