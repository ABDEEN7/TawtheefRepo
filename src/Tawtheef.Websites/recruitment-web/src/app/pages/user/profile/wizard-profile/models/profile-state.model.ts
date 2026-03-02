import {Degree} from "./degree.model";
import {Attachment} from './attachment.model';
import {Language} from './language.model';
import {Experience, TrainingCourse} from './experience.model';
import {Achievement} from './achievement.model';
import {DropdownOptionVM} from '../../../../../shared/models/dropdown-options.model';
import {PhoneNumber} from './phone-number.model';
import {Skill} from './skill.model';
import {CountryDto, CountryVM} from '../services/profile-lookups.service';
import {GUID} from '../../../../../shared/types/guid.type';

export interface UploadedFileRef {
  resourceId: GUID | string;
  resourceName: string;
  url?: string | null;
  file?: File | null;
}
export interface ProfileState {
  provider: 'Google' | 'QatarPass' | 'QatarResidentOtp'
  // prereq
  candidateType?: DropdownOptionVM;
  targetEntity?: DropdownOptionVM;
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
  qidExpiry?: string | null;
  nationality?: DropdownOptionVM | null;
  gender?: DropdownOptionVM;
  religion?: DropdownOptionVM;
  marital?: DropdownOptionVM;
  dob?: string;

  hasDisability: boolean;
  disabilityDetails?: string | null;
  sponsorType?: DropdownOptionVM | null;
  sponsorEmployerName?: string | null;
  sponsorEmployerNumber?: string | null;
  sponsorQidExpiry?: string | null;
  sponsorCardName?: string | null;
  sponsorCard?: UploadedFileRef | null;

  // contact
  country?: CountryVM;
  address?: string;
  phone?: PhoneNumber | null;
  phoneVerified: boolean | null;
  email?: string | null;
  emailVerified: boolean | null;
  interviewPlace?: DropdownOptionVM;
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
  achievements: Achievement[];
  skills: Skill[];
  languages: Language[];
  attachments: Attachment[];

  // ui
  available: boolean;
  avatarUrl?: string;
  isKawaderQid?: boolean;
}
