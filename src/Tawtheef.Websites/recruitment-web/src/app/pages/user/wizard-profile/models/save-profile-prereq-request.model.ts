export interface SaveProfilePrereqRequestModel {
  submit: boolean;
  candidateTypeId: string;
  targetEntityId: string;
  cvFileName?: string | null;
  idFileName?: string | null;
  birthCertificateFileName?: string | null;
  marriageCertificateFileName?: string | null;
}
