export interface SaveProfilePrereqRequestModel {
  submit: boolean;
  candidateTypeId: string;
  targetEntityId: string;
  officeId?: string | null;
  qidExpiry?: string | null;
  cvFileName?: string | null;
  idFileName?: string | null;
  birthCertificateFileName?: string | null;
  marriageCertificateFileName?: string | null;
}
