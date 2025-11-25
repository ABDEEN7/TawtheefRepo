import {Degree} from "./degree.model";
import {Attachment} from './attachment.model';
import {Language} from './language.model';
import {Experience} from './experience.model';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {PhoneNumber} from './phone-number.model';

export interface UploadedFileRef {
  resourceId: string;
  resourceName: string;
}
export interface ProfileState {
  // prereq
  candidateType?: dropdownOptionsModel;
  targetEntity?: dropdownOptionsModel;
  cvName?: string | null;
  cvFile?: UploadedFileRef | null;
  idName?: string | null;
  idFile?: UploadedFileRef  | null;
  birthCertificateName?: string | null;
  birthCertificateFile?: UploadedFileRef  | null;
  marriageCertificateName?: string | null;
  marriageCertificateFile?: UploadedFileRef  | null;

  // personal
  fullName?: string;
  fullNameEn?: string;
  qid?: string;
  nationality?: dropdownOptionsModel;
  gender?: dropdownOptionsModel;
  religion?: dropdownOptionsModel;
  marital?: dropdownOptionsModel;
  children?: number;
  dob?: string;

  hasDisability: boolean;
  disabilityDetails?: string | null;
  sponsorType?: any | null;
  sponsorEmployerName?: string | null;
  sponsorEmployerNumber?: string | null;
  sponsorCardName?: string | null;
  sponsorCardFile?: UploadedFileRef | null;

  // contact
  country?: dropdownOptionsModel;
  dialCode?: string;
  address?: string;
  phone?: PhoneNumber | null;
  phoneVerified: boolean;
  email?: string;
  emailVerified: boolean;
  interviewPlace?: string;
  naZone?: string;
  naStreet?: string;
  naBuilding?: string;
  naUnit?: string;
  naFileName?: string;
  naFile?: UploadedFileRef | null;

  // collections
  degrees: Degree[];
  experiences: Experience[];
  courses: Experience[];
  achievements: Experience[];
  skills: string[];
  languages: Language[];
  attachments: Attachment[];

  // ui
  available: boolean;
  avatarUrl?: string;
}
