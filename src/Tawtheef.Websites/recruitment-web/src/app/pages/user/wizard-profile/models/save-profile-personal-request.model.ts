import {UploadedFileRef} from './profile-state.model';

export interface SaveProfilePersonalRequestDto {
  submit: boolean;

  fullName: string | null;
  fullNameEn: string | null;
  nationalNumber: string | null;
  birthDate: string | null;

  nationalityId: string | null;
  genderId: string | null;
  religionId: string | null;
  maritalStatusId: string | null;
  childrenCount: number | null;

  hasDisability: boolean;
  disabilityDetails: string | null;

  sponsorEmployerName: string | null;
  sponsorEmployerNumber: string | null;
  sponsorTypeId: string | null;
  sponsorCard: UploadedFileRef | null;
}
