export interface SaveProfilePrereqRequestModel {
  submit: boolean;
  candidateTypeId: string;
  targetEntityId: string;

  cvResourceId: string | null;
  cvFileName: string | null;

  idResourceId: string | null;
  idFileName: string | null;

  birthCertResourceId: string | null;
  birthCertFileName: string | null;

  marriageCertResourceId: string | null;
  marriageCertFileName: string | null;
}
