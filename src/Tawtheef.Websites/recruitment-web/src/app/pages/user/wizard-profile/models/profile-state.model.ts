import {Degree} from "./degree.model";
import {Attachment} from './attachment.model';
import {Language} from './language.model';
import {Experience} from './experience.model';
import {dropdownOptionsModel} from '../../../../shared/models/dropdown-options.model';

export interface ProfileState {
  // prereq
  candidateType?: dropdownOptionsModel;
  targetEntity?: dropdownOptionsModel;
  cvName?: string;
  idName?: string;
  marriageCertificateName?: string;
  birthCertificateName?: string;

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

  // contact
  country?: dropdownOptionsModel;
  dialCode?: string;
  phone?: string;
  address?: string;
  email?: string;
  interviewPlace?: string;
  naZone?: string;
  naStreet?: string;
  naBuilding?: string;
  naUnit?: string;
  naFileName?: string;

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

  hasDisability?: boolean | null;
  disabilityDetails?: string | null;
  sponsorType?: any | null;
  sponsorEmployerName?: string | null;
  sponsorEmployerNumber?: string | null;
  sponsorCardName?: string | null;
}
