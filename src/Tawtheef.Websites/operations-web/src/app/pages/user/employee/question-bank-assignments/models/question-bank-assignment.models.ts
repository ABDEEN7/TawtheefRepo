export interface AssignmentOption {
  id?: string;
  optionTextAr?: string;
  optionTextEn?: string;
  isCorrect: boolean;
  displayOrder: number;
}

export interface AssignmentQuestion {
  itemId: string;
  questionId: string;
  revisionId: string;
  revisionNo: number;
  questionTypeId: string;
  questionTypeNameAr: string;
  questionTypeNameEn: string;
  difficultyLevelId: string;
  difficultyNameAr: string;
  difficultyNameEn: string;
  questionTextAr?: string;
  questionTextEn?: string;
  explanationAr?: string;
  explanationEn?: string;
  resourceId?: string;
  imageUrl?: string;
  statusId: string;
  statusNameAr: string;
  statusNameEn: string;
  latestDecisionId?: string;
  latestReviewNote?: string;
  latestReviewRound?: number;
  options: AssignmentOption[];
}

export interface QuestionInput {
  questionTypeId: string;
  difficultyLevelId: string;
  questionTextAr?: string;
  questionTextEn?: string;
  explanationAr?: string;
  explanationEn?: string;
  resourceId?: string | null;
  imageFile?: File;
  options: AssignmentOption[];
}

export interface MyAssignment {
  assignmentId: string;
  requestId: string;
  questionBankTypeNameAr: string;
  questionBankTypeNameEn: string;
  managementNameAr?: string;
  managementNameEn?: string;
  jobTitleNameAr?: string;
  jobTitleNameEn?: string;
  statusId: string;
  statusNameAr: string;
  statusNameEn: string;
  minimumQuestionCount: number;
  currentQuestionCount: number;
  assignedAt: string;
}

export interface AssignmentWorkspace {
  assignment: {
    id: string;
    statusId: string;
    statusNameAr: string;
    statusNameEn: string;
    minimumQuestionCount: number;
    notes?: string;
    assignedAt: string;
    questionEntryStartedAt?: string;
    questionEntryCompletedAt?: string;
  };
  request: {
    id: string;
    statusId: string;
  };
  questionBank: {
    questionBankTypeNameAr: string;
    questionBankTypeNameEn: string;
    managementNameAr?: string;
    managementNameEn?: string;
    jobTitleNameAr?: string;
    jobTitleNameEn?: string;
  };
  progress: {
    minimumQuestionCount: number;
    currentQuestionCount: number;
    remainingQuestionCount: number;
    canFinish: boolean;
  };
  questions: AssignmentQuestion[];
}

export const AssignmentStatuses = {
  assigned: '9b38c5a0-fb16-44f2-8239-cb5ee704129d',
  inProgress: '8b73c382-c04b-49f7-bb78-4bc235df872f',
  completed: 'c6cddace-de75-4a6e-8d1f-65e7e93ba4ed',
  returnedForModification: '238e3c8d-84de-4d70-8a1f-192ac93224ba',
  modificationCompleted: 'c035e63f-d866-488f-affd-9ae08a7043dd',
};

export const RequestItemStatuses = {
  draft: '19760735-e5f0-4236-9fff-8487f9f057ef',
  pendingReview: 'c5ed702c-cfca-4d7e-8bf9-98cdf15f2106',
  approved: '1914dd04-27d9-468d-b03e-813df6ce87ff',
  needsModification: 'bfa1d9c1-a284-452b-8b59-a2d5fc0a6dfe',
  rejected: '217164f8-c23d-4913-a549-16f80186f9cb',
};

export const QuestionTypes = {
  multipleChoice: '04a6e083-d38f-4838-a820-402fa4c53a39',
  trueFalse: '62d2e3f9-562d-45f1-ace4-867b929cae43',
};
