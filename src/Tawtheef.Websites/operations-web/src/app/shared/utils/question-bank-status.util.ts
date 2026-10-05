export type QuestionBankStatusSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary';

const requestSeverities: Record<string, QuestionBankStatusSeverity> = {
  '4b25e8db-92f1-4b4c-baab-756a5d1fd3f8': 'warn',
  '66fbfd41-563e-4a12-a006-8a970285f62b': 'info',
  '97c0610c-a992-43e1-b9a1-bfdbcf5b34b0': 'secondary',
  'f5cbde41-7e46-4793-953f-ebe5f8e31570': 'warn',
  '8b6d8c90-2e40-4957-ac52-bd46b0ae9726': 'success',
  '77687c70-161f-4a6c-977e-95f86b37d724': 'secondary',
  'fa2e079c-9361-4e5a-9cbd-39dcb73a1520': 'danger',
};

const assignmentSeverities: Record<string, QuestionBankStatusSeverity> = {
  '9b38c5a0-fb16-44f2-8239-cb5ee704129d': 'warn',
  '8b73c382-c04b-49f7-bb78-4bc235df872f': 'info',
  'c6cddace-de75-4a6e-8d1f-65e7e93ba4ed': 'secondary',
  '238e3c8d-84de-4d70-8a1f-192ac93224ba': 'warn',
  'c035e63f-d866-488f-affd-9ae08a7043dd': 'secondary',
  'a8ca906a-e1a3-44c9-87c1-8b4ec6192ab0': 'success',
  'a4851953-f48a-4aa9-afb9-b51dfbc1e4d8': 'secondary',
};

const requestItemSeverities: Record<string, QuestionBankStatusSeverity> = {
  '19760735-e5f0-4236-9fff-8487f9f057ef': 'secondary',
  'c5ed702c-cfca-4d7e-8bf9-98cdf15f2106': 'info',
  '1914dd04-27d9-468d-b03e-813df6ce87ff': 'success',
  'bfa1d9c1-a284-452b-8b59-a2d5fc0a6dfe': 'warn',
  '217164f8-c23d-4913-a549-16f80186f9cb': 'danger',
  'f48eedf5-4fe9-4ded-931b-c76bd5c9ba80': 'secondary',
};

export const questionBankRequestStatusSeverity = (statusId: string): QuestionBankStatusSeverity =>
  requestSeverities[statusId.toLowerCase()] ?? 'secondary';

export const questionBankAssignmentStatusSeverity = (
  statusId: string,
): QuestionBankStatusSeverity => assignmentSeverities[statusId.toLowerCase()] ?? 'secondary';

export const questionBankRequestItemStatusSeverity = (
  statusId: string,
): QuestionBankStatusSeverity => requestItemSeverities[statusId.toLowerCase()] ?? 'secondary';
