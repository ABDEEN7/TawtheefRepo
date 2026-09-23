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
  statusId: string;
  statusNameAr: string;
  statusNameEn: string;
  options: AssignmentOption[];
}

export interface QuestionInput {
  questionTypeId: string;
  difficultyLevelId: string;
  questionTextAr?: string;
  questionTextEn?: string;
  explanationAr?: string;
  explanationEn?: string;
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
};

export const QuestionTypes = {
  multipleChoice: '04a6e083-d38f-4838-a820-402fa4c53a39',
  trueFalse: '62d2e3f9-562d-45f1-ace4-867b929cae43',
};
