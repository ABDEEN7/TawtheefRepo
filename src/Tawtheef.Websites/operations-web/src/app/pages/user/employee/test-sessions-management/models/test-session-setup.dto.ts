export type TestSessionGenderFilter = 'Male' | 'Female' | null;
export type TestSessionNationalityFilter = 'Qatari' | 'NonQatari' | null;

export interface TestSessionSetupDto {
  testSessionId: string;
  examId: string;
  testSlotId: string;
  startTime: string;
  endTime: string;
  genderFilter: TestSessionGenderFilter;
  nationalityFilter: TestSessionNationalityFilter;
  invitationIds: string[];
  candidateCount: number;
  statusId: string;
}

export interface SaveTestSessionSetupDto {
  testSessionId?: string;
  examId: string;
  testSlotId: string;
  startTime: string;
  endTime: string;
  genderFilter: TestSessionGenderFilter;
  nationalityFilter: TestSessionNationalityFilter;
  invitationIds: string[];
  sendToApprove: boolean;
}
