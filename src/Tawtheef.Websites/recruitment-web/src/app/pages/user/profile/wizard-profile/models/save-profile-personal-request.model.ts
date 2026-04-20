export interface SaveProfilePersonalRequestDto {
  submit: boolean;

  fullNameAr: string | null;
  fullNameEn: string | null;
  nationalNumber: string | null;
  qidExpiry: string | null;
  birthDate: string | null;

  nationalityId: string | null;
  genderId: string | null;
  religionId: string | null;
  maritalStatusId: string | null;

  hasDisability: boolean;
  disabilityDetails?: string | null;

  sponsorEmployerName: string | null;
  sponsorEmployerNumber: string | null;
  sponsorQidExpiry: string | null;
  sponsorTypeId: string | null;
  sponsorCardFileName: string | null;
}
