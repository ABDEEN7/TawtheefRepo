import {Degree} from "./degree.model";
import {Attachment} from './attachment.model';
import {Language} from './language.model';
import {Experience, TrainingCourse} from './experience.model';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';
import {PhoneNumber} from './phone-number.model';
import {Skill} from './skill.model';

export interface UploadedFileRef {
  resourceId: string;
  resourceName: string;
  url?: string | null;
  file?: File | null;
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
  fullNameAr?: string;
  fullNameEn?: string;
  qid?: string | null;
  nationality?: dropdownOptionsModel | null;
  gender?: dropdownOptionsModel;
  religion?: dropdownOptionsModel;
  marital?: dropdownOptionsModel;
  children?: number;
  dob?: string;

  hasDisability: boolean;
  disabilityDetails?: string | null;
  sponsorType?: dropdownOptionsModel | null;
  sponsorEmployerName?: string | null;
  sponsorEmployerNumber?: string | null;
  sponsorCardName?: string | null;
  sponsorCardFile?: UploadedFileRef | null;

  // contact
  country?: dropdownOptionsModel;
  address?: string;
  phone?: PhoneNumber | null;
  phoneVerified: boolean | null;
  email?: string | null;
  emailVerified: boolean | null;
  interviewPlace?: dropdownOptionsModel;
  naZone?: string | null;
  naStreet?: string | null;
  naBuilding?: string | null;
  naUnit?: string | null;
  naFileName?: string | null;
  naFile?: UploadedFileRef | null;

  // collections
  degrees: Degree[];
  experiences: Experience[];
  courses: TrainingCourse[];
  skills: Skill[];
  languages: Language[];
  attachments: Attachment[];

  // ui
  available: boolean;
  avatarUrl?: string;
}
